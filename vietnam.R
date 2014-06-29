library(plyr)

companies <- read.csv('output/companies.csv')
vietnam <- sum(all$bidder.country == 'Vietnam' | grepl('Vietnam', all$original.company.country, ignore.case = TRUE) | grepl('Vietnam', all$country, ignore.case = TRUE))

group.companies <- function(df){
  columns <- c(
    "contract.uri",
    "bidder.name",
    "project.name",
    "contract.number",
    "bid.status",
    "bidder.country",
  # "original.company.name",
  # "original.company.country",
  # "opencorporates.company.name",
  # "opencorporates.company.uri",
    "address",
    "country",
    "duration",
    "date.signature",
    "score.financial",
    "score.technical",
    "score.final",
    "original.price.opening",
    "original.price.evaluated",
    "price.opening.currency",
    "price.opening.amount",
    "price.evaluated.currency",
    "price.evaluated.amount",
    "method.procurement",
    "method.selection",
    "scope",
    "name",
    "reason.rejection",
    "small.contract.notice",
    "ranking.final",
    "original.price.contract",
    "price.contract.currency",
    "price.contract.amount"
  )
  df[1,columns]
}

companies.subset <- companies[vietnam,]
bidders.subset <- ddply(companies.subset, c('contract.uri','bidder.name','original.company.name'), group.companies)

write.csv(companies.subset, 'output/vietnam-companies.csv', row.names = FALSE)
write.csv(bidders.subset, 'output/vietnam-bidders.csv', row.names = FALSE)
