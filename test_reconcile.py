import nose.tools as n

import reconcile as r

testcases_split = [
    ('Kocks Consult GmbH# (Germany) in association with #BCL# (Bangladesh)and #Kyrgyzdortransproject#',
        [('Kocks Consult GmbH', 'Germany'), ('BCL', 'Bangladesh'), ('Kyrgyzdortransproject', None)]),
    ('Consortium of: ABG S.A. (Lead Partner) and Asseco Poland S.A.',
        [('ABG S.A.', None), ('Asseco Poland S.A.', None)]),
    ('JV of GVG (GERMANY) (Leading partner) and Swedish Public EmploymentService',
        [('GVG', 'GERMANY'), ('Swedish Public EmploymentService', None)]),
    ('PT. Indomas Mulia JV PT. Prismaita Cipta Kreasi',
        [('Indomas Mulia', None), ('Prismaita Cipta Kreasi', None)]),
    ('JV: BUDIMEX S.A.- Leader; FERROVIAL AGROMAN S.A. - Partner',
        [('BUDIMEX S.A.', None), ('FERROVIAL AGROMAN S.A.', None)]),
    ('UTE Vesalius Pharma S.A.S. y Rhydburg Pharmaceuticals Limited',
        [('UTE Vesalius Pharma S.A.S.', None), ('Rhydburg Pharmaceuticals Limited', None)]),
    ('Joint venture of OJSC RB&E OF GAVAR and Chanaparh LLC',
        [('OJSC RB&E OF GAVAR', None), ('Chanaparh LLC', None)]),
    ('Joint Venture of STEGET srl (Italy),ESTIA srl (Italy), SWSEngineeering S.P.A. (Italy), GDP Consultants (Italy), Studio SANI (Italy)',
        [('STEGET srl', 'Italy'), ('ESTIA srl', 'Italy'), ('SWSEngineeering S.P.A.', 'Italy'), ('GDP Consultants', 'Italy'), ('Studio SANI', 'Italy')]),
]

testcases_respell = [
    ('PT. Indomas Mulia', ('Indomas Mulia', None)),
    ('Ltd «Navovar-2003»', ('Navovar-2003 Ltd', None)),
    ('M/s Haffkine Bio-Pharmaceutical Corporation Ltd', ('Haffkine Bio-Pharmaceutical Corporation Ltd', None)),
    ('#BCL# (Bangladesh', ('BCL', 'Bangladesh')),
    ("M/S Nyakirang'ani Construction Ltd", ("Nyakirang'ani Construction Ltd", None)),
]

def check_bid(o, e):
    n.assert_list_equal(list(r.bid(o)), e)

def test():
    testcases = testcases_split + [(a,[b]) for a,b in testcases_respell]
    for observed, expected in testcases:
        yield check_bid, observed, expected
