1. inbuiltTablesChanges.sql
	It contains script to create tables/columns used for internal purpose.
	It also inserts data in inbuilt tables for internal purpose or generic features like ApplicationSettingConfiguation, Business Rules etc.

2. ModelScript.sql
	Every launch will create a ModelScript file with name TurantoVersion and launch date time stamp (in UTC).
	File contains the script required for db changes according to the model changes done in Turanto. (e.g. adding/deleting/changing  Entity, Property or association)
	
3. CreateFunctions.sql (Recycle-Bin, Soft Delete)
	It has script to generate functions for each entity to filter IsDeleted=True and IsDeleted=False. 
	e.g. fn_SessionEventsList(@includeDeleted bit)

4. TenantFunctions.sql
	It has script to generate functions for each tenant affected entity 
	e.g. fn_SessionEvents(@tenantID bit,@includeDeleted bit) - First parameter is tenantID and Second parameter is includeDeleted.

5. TenantSchemaAndViews.sql
	Script to create schema and views for each tenant. e.g. organization1.vw_client(1,0)

6. TenantScript.sql
	Script to create tenant related functions, this is compulsory to execute if application has tenant security.
	
Note: 
1. If your application is using Tenant Security then use TenantFunctions.Sql (no need of using CreateFunctions.sql)
2. You should always execute inbuiltTablesChanges.sql and ModelScript.sql to update tables for new features and fixes.
3. Execution Sequence  1, 2,3/4, 5, 6
4. Script 3,4 and 5 are optional, they are helpful in case of SSRS reporting.
5. Script 6 is compulsory to execute if application has tenant security.