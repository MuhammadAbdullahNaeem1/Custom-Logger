CREATE TABLE LoginLogs (
    LogsID INT,
    ApplicationName VARCHAR(50),
    LogType VARCHAR(50),
    InnerException VARCHAR(255),
    Message VARCHAR(255),
    CreatedOn DATE,
    CreatedBy INT,
    ModifiedOn DATE,
    ModifiedBy INT,
    Is_Deleted CHAR(1)
);

select *
from LoginLogs;
drop table LoginLogs;