if (!('companies' %in% ls())) {
  companies <- read.csv('output/companies.csv')
}
columns <- c('contract.uri','bidder.name',
  'original.price.contract',
  'price.contract.currency',
  'price.contract.amount'
)
awarded <- companies[!duplicated(companies[columns]) & companies$bid.status == 'awarded',columns]

merged <- merge(companies[names(companies)[-grep('price.contract', names(companies))]],
                awarded,
                by = c('contract.uri', 'bidder.name'), all.x = TRUE)
