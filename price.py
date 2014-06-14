import pandas

companies = pandas.read_csv(os.path.join('output','companies.csv'))
companies[companies['status'] == 'awarded']
#companies
#contract
