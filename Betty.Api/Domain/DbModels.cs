using Betty.Api.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
#pragma warning disable CS8618
namespace BettyApi.Models
{

    public abstract class DictionaryBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class c_UserStoryState : DictionaryBase
    {
    }

    public class c_TaskState : DictionaryBase
    {
    }

    public class c_TaskRelationType : DictionaryBase
    {
    }

    public class c_UserState : DictionaryBase
    {
    }

    public class c_TaskUserStoryRelatedType : DictionaryBase
    {
    }

    public class c_UserStoryPriority : DictionaryBase
    {
    }

    public class c_ProjectState : DictionaryBase
    {
    }

    public class User
    {
        [SqlPrimaryKeyAttribute]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public int UserStateCode { get; set; }
    }

    public class UserCredential
    {
        public int UserCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Project
    {
        [SqlPrimaryKeyAttribute]
        public int ProjectId { get; set; }
        [SqlIgnoreUpdate]
        public int CreatedByUser { get; set; }
        [SqlIgnoreInsert]
        public int ModifiedByUser { get; set; }
        [SqlIgnoreUpdate]
        public DateTime CreatedDateTime { get; set; }
        [SqlIgnoreInsert]
        public DateTime ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int ProjectStateCode { get; set; }
    }

    public class Epic
    {
        [SqlPrimaryKeyAttribute]
        public int EpicId { get; set; }
        public int CreatedByUser { get; set; }
        [SqlIgnoreInsert]
        public int ModifiedByUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        [SqlIgnoreInsert]
        public DateTime? ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int UserStoryStateCode { get; set; }
        public int ProjectCode { get; set; }
    }

    public class UserStory
    {
        [SqlPrimaryKeyAttribute]
        public int UserStoryId { get; set; }
        public int CreatedByUser { get; set; }
        [SqlIgnoreInsert]
        public int ModifiedByUser { get; set; }
        public DateTime CreatedDateTime { get; set; }
        [SqlIgnoreInsert]
        public DateTime? ModifiedDateTime { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public int UserStoryPriorityCode { get; set; }
        public int StoryPoints { get; set; }
        public int UserStoryStateCode { get; set; }
        public int ProjectCode { get; set; }
    }

    public class Task
    {
        [SqlPrimaryKeyAttribute]
        public int TaskId { get; set; }
        public int CreatedByUserCode { get; set; }
        public int ModifiedByUserCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public int? ParentUserStoryCode { get; set; }
        public int? ParentTaskCode { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime? LastModified { get; set; }
        public DateTime Created { get; set; }
        public int TaskStateCode { get; set; }
        public int ProjectCode { get; set; }
        [SqlIgnoreAttribute]
        public User CreatedBy { get; set; }
        [SqlIgnoreAttribute]
        public User ModifiedBy { get; set; }
        [SqlIgnoreAttribute]
        public UserStory ParentUserStory { get; set; }
        [SqlIgnoreAttribute]
        public Task ParentTask { get; set; }
        [SqlIgnoreAttribute]
        public c_TaskState TaskState { get; set; }
        [SqlIgnoreAttribute]
        public Project Project { get; set; }
    }

    public class TaskRelatedByTask
    {
        public int TaskRelatedByTaskId { get; set; }
        public int TaskParentCode { get; set; }
        public int TaskCode { get; set; }
        public int TaskRelationTypeCode { get; set; }
        [SqlIgnoreAttribute]
        public Task TaskParent { get; set; }
        [SqlIgnoreAttribute]
        public Task Task { get; set; }
        [SqlIgnoreAttribute]
        public c_TaskRelationType TaskRelationType { get; set; }
    }

    public class TaskRelatedByUserStory
    {
        public int TaskRelatedByUserStoryId { get; set; }
        public int UserStoryParentCode { get; set; }
        public int TaskChildCode { get; set; }
        public int TaskRelationTypeCode { get; set; }
        [SqlIgnoreAttribute]
        public UserStory UserStoryParent { get; set; }
        [SqlIgnoreAttribute]
        public Task TaskChild { get; set; }
        [SqlIgnoreAttribute]
        public c_TaskRelationType TaskRelationType { get; set; }
    }
}
