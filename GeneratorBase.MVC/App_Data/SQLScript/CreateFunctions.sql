
---tbl_User---
IF OBJECT_ID('fn_UserList') IS NOT NULL
 DROP FUNCTION fn_UserList
 GO
 CREATE FUNCTION fn_UserList(@includeDeleted bit = 0)
 RETURNS TABLE  
 AS
 RETURN  
 select *
 from tbl_User 
 GO


---tbl_FileDocument---
IF OBJECT_ID('fn_FileDocumentList') IS NOT NULL
 DROP FUNCTION fn_FileDocumentList
 GO
 CREATE FUNCTION fn_FileDocumentList(@includeDeleted bit = 0)
 RETURNS TABLE  
 AS
 RETURN  
 select *
 from tbl_FileDocument 
 GO


---tbl_Customer---
IF OBJECT_ID('fn_CustomerList') IS NOT NULL
 DROP FUNCTION fn_CustomerList
 GO
 CREATE FUNCTION fn_CustomerList(@includeDeleted bit = 0)
 RETURNS TABLE  
 AS
 RETURN  
 select *
 from tbl_Customer 
 GO

