CREATE DATABASE `betty_db`;
use `betty_db`;

create table `c_UserStoryState`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);
create table `c_TaskState`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);
create table `c_TaskRelationType`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);
create table `c_UserState`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);
create table `c_ProjectState`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);

create table `c_PermissionType`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);

create table `c_TaskUserStoryRelatedType`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);
create table `c_UserStoryPriority`(
	`Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Name` VARCHAR(50)
);

insert into `c_PermissionType`(Id, Name) Values(1, 'Admin');

insert into `c_UserState`(Id, Name) Values(1, 'Enabled');
insert into `c_UserState`(Id, Name) Values(2, 'Disabled');

insert into `c_ProjectState`(Id, Name) Values(1, 'New');
insert into `c_ProjectState`(Id, Name) Values(2, 'Active');
insert into `c_ProjectState`(Id, Name) Values(3, 'Review');
insert into `c_ProjectState`(Id, Name) Values(4, 'Closed');

insert into `c_TaskState`(Id, Name) Values(1, 'New');
insert into `c_TaskState`(Id, Name) Values(2, 'Active');
insert into `c_TaskState`(Id, Name) Values(3, 'Review');
insert into `c_TaskState`(Id, Name) Values(4, 'Closed');

insert into `c_UserStoryState`(Id, Name) Values(1, 'New');
insert into `c_UserStoryState`(Id, Name) Values(2, 'Active');
insert into `c_UserStoryState`(Id, Name) Values(3, 'Review');
insert into `c_UserStoryState`(Id, Name) Values(4, 'Closed');

insert into `c_UserStoryPriority`(Id, Name) Values(1, 'Optional');
insert into `c_UserStoryPriority`(Id, Name) Values(2, 'Moderately Important');
insert into `c_UserStoryPriority`(Id, Name) Values(3, 'Important');
insert into `c_UserStoryPriority`(Id, Name) Values(4, 'Critical');




create table `User`(
	`UserId` INT AUTO_INCREMENT PRIMARY KEY,
    `Username` VARCHAR(50),
    `Name` VARCHAR(50),
    `Lastname` VARCHAR(50),
    `Email` VARCHAR(50),
    `UserStateCode` INT
);
alter table `User`ADD CONSTRAINT `fk_UserStateCode`
FOREIGN KEY (`UserStateCode`)
REFERENCES `c_UserState`(`Id`);

create table `UserCredential`(
	`UserCode` INT,
    `Username` VARCHAR(50),
    `Password` VARCHAR(64)
);

--	QA Test
Insert into `User`(Username,Name,Lastname,Email) Values("admin", "admin", "admin", "admin");
Insert Into `UserCredential`(UserCode, Username, Password) Values(1, "admin", '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918');

Create table `Project`(
	`ProjectId` INT AUTO_INCREMENT PRIMARY KEY,
	`CreatedByUser` INT,
    `ModifiedByUser` INT,
	`CreatedDateTime` DATETIME,
	`ModifiedDateTime` DATETIME null,
    `Title` VARCHAR(50),
    `Text` VARCHAR(50),
	`ProjectStateCode` INT
);

alter table `Project`ADD CONSTRAINT `fk_ProjectCreatedByUser`
FOREIGN KEY (`CreatedByUser`)
REFERENCES `User`(`UserId`);

alter table `Project`ADD CONSTRAINT `fk_ProjectModifiedByUser`
FOREIGN KEY (`ModifiedByUser`)
REFERENCES `User`(`UserId`);

alter table `Project`ADD CONSTRAINT `fk_ProjectStateCode`
FOREIGN KEY (`ProjectStateCode`)
REFERENCES `c_ProjectState`(`Id`);

create table`UserPermissionsByProject`(
	`UserCode` INT,
	`ProjectCode` INT,
	`PermissionTypeCode` INT
);

alter table `UserPermissionsByProject`ADD CONSTRAINT `fk_UserPermissionsByProject_ProjectCode`
FOREIGN KEY (`ProjectCode`)
REFERENCES `Project`(`ProjectId`);

alter table `UserPermissionsByProject`ADD CONSTRAINT `fk_UserPermissionsByProject_PermissionTypeCode`
FOREIGN KEY (`PermissionTypeCode`)
REFERENCES `c_PermissionType`(`Id`);

alter table `UserPermissionsByProject`ADD CONSTRAINT `fk_UserPermissionsByProject_UserCode`
FOREIGN KEY (`UserCode`)
REFERENCES `User`(`UserId`);

Create table `Epic`(
	`EpicId` INT AUTO_INCREMENT PRIMARY KEY,
	`CreatedByUser` INT,
    `ModifiedByUser` INT,
	`CreatedDateTime` DATETIME,
	`ModifiedDateTime` DATETIME null,
    `Title` VARCHAR(50),
    `Text` VARCHAR(50),
	`UserStoryStateCode` INT,
	`ProjectCode` INT
);

alter table `Epic`ADD CONSTRAINT `fk_EpicCreatedByUser`
FOREIGN KEY (`CreatedByUser`)
REFERENCES `User`(`UserId`);

alter table `Epic`ADD CONSTRAINT `fk_EpicModifiedByUser`
FOREIGN KEY (`ModifiedByUser`)
REFERENCES `User`(`UserId`);

alter table `Epic`ADD CONSTRAINT `fk_EpicProjectCode`
FOREIGN KEY (`ProjectCode`)
REFERENCES `Project`(`ProjectId`);

alter table `Epic`ADD CONSTRAINT `fk_EpicStateCode`
FOREIGN KEY (`UserStoryStateCode`)
REFERENCES `c_UserStoryState`(`Id`);


create table `UserStory`(
	`UserStoryId` INT AUTO_INCREMENT PRIMARY KEY,
    `CreatedByUser` INT,
    `ModifiedByUser` INT,
	`CreatedDateTime` DATETIME,
	`ModifiedDateTime` DATETIME null,
    `Title` VARCHAR(50),
    `Text` VARCHAR(50),
	`UserStoryPriorityCode` INT,
	`StoryPoints` INT,
    `UserStoryStateCode` INT,
	`ProjectCode` INT,
	`EpicCode`INT
);
alter table `UserStory`ADD CONSTRAINT `fk_UserStoryCreatedByUser`
FOREIGN KEY (`CreatedByUser`)
REFERENCES `User`(`UserId`);

alter table `UserStory`ADD CONSTRAINT `fk_UserStoryModifiedByUser`
FOREIGN KEY (`ModifiedByUser`)
REFERENCES `User`(`UserId`);

alter table `UserStory`ADD CONSTRAINT `fk_UserStoryStateCode`
FOREIGN KEY (`UserStoryStateCode`)
REFERENCES `c_UserStoryState`(`Id`);

alter table `UserStory`ADD CONSTRAINT `fk_UserStoryPriorityCode`
FOREIGN KEY (`UserStoryPriorityCode`)
REFERENCES `c_UserStoryPriority`(`Id`);

alter table `UserStory`ADD CONSTRAINT `fk_UserStoryProjectCode`
FOREIGN KEY (`ProjectCode`)
REFERENCES `Project`(`ProjectId`);

alter table `UserStory`ADD CONSTRAINT `fk_UserStoryEpicCode`
FOREIGN KEY (`EpicCode`)
REFERENCES `Epic`(`EpicId`);

create table `Task`(
	`TaskId` INT AUTO_INCREMENT PRIMARY KEY,
    `CreatedByUser` INT,
    `ModifiedByUser` INT,
	`ResponsibleUserCode` INT,
	`CreatedDateTime` DATETIME,
	`ModifiedDateTime` DATETIME null,
    `ParentUserStoryCode` INT null,
    `Title` VARCHAR(50),
    `Text` VARCHAR(50),
    `TaskStateCode` INT,
	`ProjectCode` INT
);
alter table `Task`ADD CONSTRAINT `fk_CreatedByUserCode`
FOREIGN KEY (`CreatedByUser`)
REFERENCES `User`(`UserId`);
alter table `Task`ADD CONSTRAINT `fk_ModifiedByUserCode`
FOREIGN KEY (`ModifiedByUser`)
REFERENCES `User`(`UserId`);

alter table `Task`ADD CONSTRAINT `fk_ParentUserStoryCode`
FOREIGN KEY (`ParentUserStoryCode`)
REFERENCES `UserStory`(`UserStoryId`);

alter table `Task`ADD CONSTRAINT `fk_TaskProjectCode`
FOREIGN KEY (`ProjectCode`)
REFERENCES `Project`(`ProjectId`);

create table `TaskRelatedByTask`(
	`TaskRelatedByTaskId` INT AUTO_INCREMENT PRIMARY KEY,
    `TaskParentCode` INT,
    `TaskCode` INT,
    `TaskRelationTypeCode` INT
);

alter table `TaskRelatedByTask`ADD CONSTRAINT `fk_TaskParentCode`
FOREIGN KEY (`TaskParentCode`)
REFERENCES `Task`(`TaskId`);

alter table `TaskRelatedByTask`ADD CONSTRAINT `fk_TaskCode`
FOREIGN KEY (`TaskCode`)
REFERENCES `Task`(`TaskId`);

alter table `TaskRelatedByTask`ADD CONSTRAINT `fk_TaskRelationTypeCode`
FOREIGN KEY (`TaskRelationTypeCode`)
REFERENCES `c_TaskRelationType`(`Id`);

create table `TaskRelatedByUserStory`(
	`TaskRelatedByUserStoryId` INT AUTO_INCREMENT PRIMARY KEY,
    `UserStoryParentCode` INT,
    `TaskChildCode` INT,
    `TaskRelationTypeCode` INT
);


