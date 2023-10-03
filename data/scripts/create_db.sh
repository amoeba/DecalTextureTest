#!/bin/sh

# Creates regions.db out of regions.csv and names.csv
# Run json_to_csv.R first

sqlite-utils create-database regions.db
sqlite-utils insert --csv regions.db regions regions.csv
sqlite-utils insert --csv regions.db names names.csv
sqlite-utils create-index regions.db regions landblock_id
sqlite-utils create-index regions.db regions landcell_id
sqlite-utils create-index regions.db names id
