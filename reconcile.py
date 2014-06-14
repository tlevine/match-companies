#!/usr/bin/env python3
from collections import defaultdict
import re
import itertools
from functools import partial
import os, csv, sys

from thready import threaded
from company import reconcile

# from datapackage import DataPackage
# COUNTRY = '(%s)' % re.sub(r' ?[\(\)] ?', '|', '|'.join(row['Name'] for row in DataPackage('http://data.okfn.org/data/core/country-list/').data)).replace('||','|')
COUNTRY = re.compile(r"^(?:.*[^a-z])?(Afghanistan|Åland Islands|Albania|Algeria|American Samoa|Andorra|Angola|Anguilla|Antarctica|Antigua and Barbuda|Argentina|Armenia|Aruba|Australia|Austria|Azerbaijan|Bahamas|Bahrain|Bangladesh|Barbados|Belarus|Belgium|Belize|Benin|Bermuda|Bhutan|Bolivia, Plurinational State of|Bonaire, Sint Eustatius and Saba|Bosnia and Herzegovina|Botswana|Bouvet Island|Brazil|British Indian Ocean Territory|Brunei Darussalam|Bulgaria|Burkina Faso|Burundi|Cambodia|Cameroon|Canada|Cape Verde|Cayman Islands|Central African Republic|Chad|Chile|China|Christmas Island|Cocos|Keeling|Islands|Colombia|Comoros|Congo|Congo, the Democratic Republic of the|Cook Islands|Costa Rica|Côte d'Ivoire|Croatia|Cuba|Curaçao|Cyprus|Czech Republic|Denmark|Djibouti|Dominica|Dominican Republic|Ecuador|Egypt|El Salvador|Equatorial Guinea|Eritrea|Estonia|Ethiopia|Falkland Islands|Malvinas|Faroe Islands|Fiji|Finland|France|French Guiana|French Polynesia|French Southern Territories|Gabon|Gambia|Georgia|Germany|Ghana|Gibraltar|Greece|Greenland|Grenada|Guadeloupe|Guam|Guatemala|Guernsey|Guinea|Guinea-Bissau|Guyana|Haiti|Heard Island and McDonald Mcdonald Islands|Holy See|Vatican City State|Honduras|Hong Kong|Hungary|Iceland|India|Indonesia|Iran, Islamic Republic of|Iraq|Ireland|Isle of Man|Israel|Italy|Jamaica|Japan|Jersey|Jordan|Kazakhstan|Kenya|Kiribati|Korea, Democratic People's Republic of|Korea, Republic of|Kuwait|Kyrgyzstan|Lao People's Democratic Republic|Latvia|Lebanon|Lesotho|Liberia|Libya|Liechtenstein|Lithuania|Luxembourg|Macao|Macedonia, the Former Yugoslav Republic of|Madagascar|Malawi|Malaysia|Maldives|Mali|Malta|Marshall Islands|Martinique|Mauritania|Mauritius|Mayotte|Mexico|Micronesia, Federated States of|Moldova, Republic of|Monaco|Mongolia|Montenegro|Montserrat|Morocco|Mozambique|Myanmar|Namibia|Nauru|Nepal|Netherlands|New Caledonia|New Zealand|Nicaragua|Niger|Nigeria|Niue|Norfolk Island|Northern Mariana Islands|Norway|Oman|Pakistan|Palau|Palestine, State of|Panama|Papua New Guinea|Paraguay|Peru|Philippines|Pitcairn|Poland|Portugal|Puerto Rico|Qatar|Réunion|Romania|Russian Federation|Rwanda|Saint Barthélemy|Saint Helena, Ascension and Tristan da Cunha|Saint Kitts and Nevis|Saint Lucia|Saint Martin|French part|Saint Pierre and Miquelon|Saint Vincent and the Grenadines|Samoa|San Marino|Sao Tome and Principe|Saudi Arabia|Senegal|Serbia|Seychelles|Sierra Leone|Singapore|Sint Maarten|Dutch part|Slovakia|Slovenia|Solomon Islands|Somalia|South Africa|South Georgia and the South Sandwich Islands|South Sudan|Spain|Sri Lanka|Sudan|Suriname|Svalbard and Jan Mayen|Swaziland|Sweden|Switzerland|Syrian Arab Republic|Taiwan, Province of China|Tajikistan|Tanzania, United Republic of|Thailand|Timor-Leste|Togo|Tokelau|Tonga|Trinidad and Tobago|Tunisia|Turkey|Turkmenistan|Turks and Caicos Islands|Tuvalu|Uganda|Ukraine|United Arab Emirates|United Kingdom|United States|United States Minor Outlying Islands|Uruguay|Uzbekistan|Vanuatu|Venezuela, Bolivarian Republic of|Viet Nam|Virgin Islands, British|Virgin Islands, U.S.|Wallis and Futuna|Western Sahara|Yemen|Zambia|Zimbabwe)", flags = re.IGNORECASE)

MATCHNAMES = [
    (r'^address$', 'address'),
    (r'^name$', 'name'),
    ('opening.*price', 'price.opening'),
    ('price.*opening', 'price.opening'),
    ('evaluat(?:ed|ion).*price', 'price.evaluated'),
    ('country', 'country'),
    (r'reason.*rejection', 'reason.rejection'),
    (r'contract ?price', 'price.contract'),
    ('duration', 'duration'),
    (r'method.*procurement', 'method.procurement'),
    ('scope', 'scope'),
    (r'signature.*date', 'date.signature'),
    (r'technical.*score', 'score.technical'),
    (r'financial.*score', 'score.financial'),
    (r'final.*score', 'score.final'),
    (r'final ?ranking', 'ranking.final'),
    (r'small ?contract', 'small.contract.notice'),
    (r'method.*procurement', 'method.procurement'),
    (r'method.*selection', 'method.selection'),
    (r'selection.*method', 'method.selection'),
]

def profile():
    raw_bids = defaultdict(lambda: [])
    for row in csv.DictReader(open(os.path.join('pagedata','company-profile.csv'))):
        raw_bids[row['CompanyId']].append((row['ProfileKey'],row['ProfileValue']))

    for contractid, raw_bid in raw_bids.items():
        bid = {}
        for match, name in MATCHNAMES:
            for key, value in raw_bid:
                if re.search(match, key, flags = re.IGNORECASE):
                    bid[name] = value
        yield contractid, bid

def ask(writer, args):
    data, query = args
    if query != '':
        results = reconcile(None, query)
        if len(results) > 0:
            name = results[0]['name']
            uri = results[0]['uri']
        else:
            name = uri = None
        data.update({
            'original.company.name': query,
            'opencorporates.company.name': name,
            'opencorporates.company.uri': uri,
        })
        data = convert_money(data)
        writer.writerow(data)

def strip(x:str) -> str:
    return x.strip(' \r\n,-;:()#')
def split(company_name:str) -> iter:
    '''
    Split up a company name into one or more real company names.
    '''
    _split = r'(?:in association with|[^a-z]and | JV |Leader|Partner| y |,)'
    _remove = r'(?:(?:^JV | JV )(?:of )?| JV$|\(?Lead(?:ing)? partner\)?|Consortium of:? ?|Leading|Partner|^JV:? ?|Joint venture(?: of)?|^M\/?S )'
    if re.search(r'(?:in association with|Consortium of|JV |^JV|joint venture| y )', company_name, flags = re.IGNORECASE):
        messy = re.split(_split, company_name, flags = re.IGNORECASE)
        name_and_country = filter(None, map(lambda x: strip(re.sub(_remove, '', x, flags = re.IGNORECASE)), messy))
    else:
        name_and_country = [company_name]
    for x in name_and_country:
        m = re.match(r'([^\(]+)\(([^\)]+)\)? *$', x)
        if m and not re.search(r'lead|partner', m.group(2), flags = re.IGNORECASE):
            yield m.group(1), m.group(2)
        else:
            yield x, None

def respell(company_name:str) -> str:
    n = str(company_name)
    n = re.sub(r'^Ltd ?(.*)$', r'\1 Ltd', n, flags = re.IGNORECASE)
    n = re.sub(r'\([^)]*$', '', n)
    n = strip(re.sub(r'(?:M/[Ss]|«|»|^P[tT].? )', '', n))
    return n

def bids(name):
    for company, country in split(name):
        if country == None:
            s = re.search(COUNTRY, company)
            if s != None:
                country = s.groups()[0]
        elif not re.search(COUNTRY, country):
            country = None
        yield respell(company), country

def args(reader):
    profile_data = dict(profile())
    for bid in reader:
        bid_data = {
            'project.name': bid['ProjectName'],
            'contract.uri': bid['Link'],
            'contract.number': bid['ContractNo'],
            'bidder.name': bid['Name'],
            'bidder.country': bid['Country'],
            'bid.status': bid['Status'],
        }
        for company, country in bids(bid['Name']):
            company_data = dict(bid_data)
            company_data['original.company.country'] = country
            company_data.update(profile_data.get(bid['CompanyId'], {}))
            yield company_data, company

def main():
    fieldnames = [
        'project.name',
        'contract.uri',
        'contract.number',
        'bidder.name',
        'bid.status',
        'bidder.country',
        'original.company.name',
        'original.company.country',
        'opencorporates.company.name',
        'opencorporates.company.uri',
        'address',
        'country',
        'duration',
        'date.signature',
        'score.financial',
        'score.technical',
        'score.final',
        'original.price.opening',
        'original.price.evaluated',
        'original.price.contract',
        'price.opening.currency',
        'price.opening.amount',
        'price.evaluated.currency',
        'price.evaluated.amount',
        'price.contract.currency',
        'price.contract.amount',
        'method.procurement',
        'method.selection',
        'scope',
        'name',
        'reason.rejection',
        'small.contract.notice',
        'ranking.final',
    ]
    reader = csv.DictReader(open(os.path.join('pagedata', 'company.csv')))
    writer = csv.DictWriter(sys.stdout, fieldnames = fieldnames)
    writer.writeheader()
    threaded(args(reader), partial(ask, writer), num_threads = 30)

def money(raw):
    'If there are multiple amounts in different currencies, take the first one.'
    match = re.match(r'^[^A-Z]*([A-Z]{3})[^0-9]*([0-9,]+)[^0-9,]*', raw)
    if match:
        currency = match.group(1)
        amount = float(match.group(2).replace(',',''))
    else:
        currency = amount = None
    return currency, amount

def convert_money(row:dict) -> dict:
    for field in ['opening','evaluated','contract']:
        row['price.%s.currency' % field], row['price.%s.currency' % field] = money(row['original.price.%' % field])
    return row

if __name__ == '__main__':
    main()
