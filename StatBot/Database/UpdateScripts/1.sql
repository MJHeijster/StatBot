ALTER TABLE Users
Add OverrideName varchar(max)
UPDATE [Database]
SET Version = 1;