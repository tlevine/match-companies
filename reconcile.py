#!/usr/bin/env python3
import re
import itertools
from functools import partial
import os, csv, sys

from thready import threaded
from company import reconcile

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
        if m and not re.match(r'lead|partner', m.group(2), flags = re.IGNORECASE):
            yield m.group(1), m.group(2)
        else:
            yield x, None

def respell(company_name:str) -> str:
    n = str(company_name)
    n = re.sub(r'^Ltd ?(.*)$', r'\1 Ltd', n, flags = re.IGNORECASE)
    n = re.sub(r'\([^)]*$', '', n)
    n = strip(re.sub(r'(?:M/[Ss]|«|»|^P[tT].? )', '', n))
    return n

def bid(name):
    for company, country in split(name):
        yield respell(company), country

def args(reader):
    for bid in reader:
        bid_data = {
            'project.name': bid['ProjectName'],
            'contract.uri': bid['Link'],
            'contract.number': bid['ContractNo'],
            'bidder.name': bid['Name'],
            'bidder.country': bid['Country'],
            'bid.status': bid['Status'],
        }
        for company, country in split(bid['Name']):
            company_data = dict(bid_data)
            company_data['original.company.country'] = country
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
    ]
    reader = csv.DictReader(open(os.path.join('pagedata', 'company.csv')))
    writer = csv.DictWriter(sys.stdout, fieldnames = fieldnames)
    writer.writeheader()
    threaded(args(reader), partial(ask, writer), num_threads = 30)

if __name__ == '__main__':
    main()
