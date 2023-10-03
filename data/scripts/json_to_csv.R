library(jsonlite)
library(dplyr)
library(sys)

json <- read_json("regions.json")

# TODO: The paste below isn't quite handling things right
#       i.e., We need to handle hierarchical names
regions_tbl <- do.call(rbind.data.frame, lapply(names(json), function(lb) {
  inner_result <- do.call(rbind, lapply(names(json[[lb]]), function(lc) {
    data.frame(
      landblock_hex = lb,
      landcell_hex = lc,
      # FIXME: Break into two columms
      name = paste(json[[lb]][[lc]], collapse = ", ")
    )
  }))

  # attempt to clean `out`
  if (length(unique(inner_result$name)) == 1) {
    inner_result$landcell_hex <- NA
    inner_result <- unique(inner_result)
    stopifnot(nrow(inner_result) == 1)
  }

  inner_result
}))

# convert to numeric
regions_tbl$landblock_id <- strtoi(regions_tbl$landblock_hex, base = 16L)
regions_tbl$landcell_id <- strtoi(regions_tbl$landcell_hex, base = 16L)

# generate names table using factor()
unique_names <- factor(regions_tbl$name)
regions_tbl$name_id <- as.numeric(unique_names)

names_tbl <- data.frame(
  id = as.numeric(unique(unique_names)),
  name = levels(unique_names)
)

# drop un-used columns
regions_tbl |>
  select(-name) -> regions_tbl

# export to csv so we can import to SQLite on the command line
write.csv(regions_tbl, "regions.csv", row.names = FALSE)
write.csv(names_tbl, "names.csv", row.names = FALSE)


# sqlite-utils create-database regions.db
# sqlite-utils insert --csv regions.db regions regions.csv
# sqlite-utils insert --csv regions.db names names.csv
