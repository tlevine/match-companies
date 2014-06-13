#!/usr/bin/env python3
import re
import itertools
from functools import partial
import os, csv, sys

from thready import threaded
from company import reconcile

def ask(writer, args):
    bid_data, query = args
    if query != '':
        results = reconcile(None, query)
        if len(results) > 0:
            name = results[0]['name']
            uri = results[0]['uri']
        else:
            name = uri = None
        bid_data.update({
            'original.company.name': query,
            'opencorporates.company.name': name,
            'opencorporates.company.uri': uri,
        })
        writer.writerow(bid_data)

def strip(x:str) -> str:
    return x.strip(' \r\n,-;:()#')
def split(company_name:str) -> list:
    '''
    Split up a company name into one or more real company names.
    '''
    _split = r'(?:in association with|[^a-z]and | JV |Leader|Partner| y )'
    _remove = r'(?:(?:^JV | JV )(?:of )?| JV$|\(?Lead(?:ing)? partner\)?|Consortium of:? ?|Leading|Partner|^JV:? ?|Joint venture(?: of)?|^M\/?S |\([^\)]+\))'
    if re.search(r'(?:in association with|Consortium of|JV |^JV|joint venture| y )', company_name, flags = re.IGNORECASE):
        messy = re.split(_split, company_name, flags = re.IGNORECASE)
        return list(filter(None, map(lambda x: strip(re.sub(_remove, '', x, flags = re.IGNORECASE)), messy)))
    else:
        return [company_name]

def respell(company_name:str) -> str:
    n = str(company_name)
    n = re.sub(r'^Ltd ?(.*)$', r'\1 Ltd', n, flags = re.IGNORECASE)
 #  n += ')' * (n.count('(') - n.count(')')) # Balance parantheses
    n = re.sub(r'\([^)]*$', '', n)
    n = strip(re.sub(r'(?:M/[Ss]|«|»)', '', n))
    return n

def args(reader):
    for bid in reader:
        bid_data = {
            'project.uri': bid['Link'],
            'project.name': bid['ProjectName'],
            'contract.number': bid['ContractNo'],
            'bid.status': bid['Status'],
            'bidder.name': bid['Name'],
            'bidder.country': bid['Country'],
        }
        for company in split(bid['Name']):
            yield bid_data, company

def main():
    fieldnames = [
        'project.uri',
        'project.name',
        'contract.number',
        'bid.uri',
        'bid.status',
        'bidder.name',
        'bidder.country',
        'original.company.name',
        'opencorporates.company.name',
        'opencorporates.company.uri',
    ]
    reader = csv.DictReader(open(os.path.join('pagedata', 'company.csv')))
    writer = csv.DictWriter(sys.stdout, fieldnames = fieldnames)
    writer.writeheader()
    threaded(args(reader), partial(ask, writer), num_threads = 30)

if __name__ == '__main__':
    main()
