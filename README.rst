The present package produces a data table about the entities who have bid on
contracts for projects funded by the World Bank Group.

Data dictionary
=============================
Within any project funded by the World Bank, many different contracts may be
involved. Several different bidders will bid on each contract. Bidders are
usually single companies, but they are sometimes joint ventures of multiple
companies. Each row in the "companies.csv" table corresponds to a company
inside a bidder inside a contract inside a project.

We have the following fields for each company.

project.name
    Name of the project that the company bid on
contract.uri
    Page on the World Bank website for the contract that the company bid on
contract.number
    Identification number for the contract
bidder.name
    This is the name of the bidder. This is not necessarily a company because
    a bidder can be a joint venture of multiple companies.
bidder.country
    Country for the bidder
bid.status
    Bids are "awarded" (evaluated and chosen) "evaluated" (evaluated and not chosen)
    or "rejected" (rejected before evaluation for not following formal bid procedures).
original.company.name
    Name of the company as listed on the World Bank website
original.company.country
    Country of the company as listed on the World Bank website
opencorporates.company.name
    Original company names were reconciled with the Open Corporates reconciliation API.
    If a match was found, this field contains the Open Corporates name; otherwise,
    this field is empty.
opencorporates.company.uri
    Original company names were reconciled with the Open Corporates reconciliation API.
    If a match was found, this field contains the Open Corporates URI; otherwise,
    this field is empty.
address
    Dunno
country
    Separate field for the country of the bidder
duration
    Dunno
date.signature
    Dunno
score.financial
    Dunno
score.technical
    Dunno
score.final
    Dunno
original.price.opening
    Raw text for the opening price
original.price.evaluated
    Raw text for the evaluated price
original.price.contract
    Raw text for the contract price
price.opening.currency
    Currency of the opening price
price.opening.amount
    Numeric opening price
price.evaluated.currency
    Currency of the evaluated price
price.evaluated.amount
    Numeric evaluated price
price.contract.currency
    Currency of the contract price
price.contract.amount
    Numeric contract price
method.procurement
    ?
method.selection
    ?
scope
    ?
name
    ?
reason.rejection
    ?
small.contract.notice
    ?
ranking.final
    ?

Maxime Rusaev produced some of these fields, (in the ``./scraper`` directory.)
and then Thomas Levine split those up further (in the ``reconcile.py`` file).

Acquiring the resulting data
===================================
The resulting table is called "companies.csv".

You can access the resulting dataset at
http://small.dada.pink/match-companies-output/companies.csv
or by running ::

    ./reconcile.py
