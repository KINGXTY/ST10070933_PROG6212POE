Final POE for PROG6212 
In order to run this project go into the appsettings file and replace the database name in this link "Server=(localdb)\\MSSQLLocalDB;Database=CMSPOE;Trusted_Connection=true;Trust Server Certificate=True" with your own databse name.
The delete the files in the migration folder 
open up the package manager console and run the following commands 
"Add-Migration InitialMigration"
and then run "Update-database"
Then the project will run as intended 
