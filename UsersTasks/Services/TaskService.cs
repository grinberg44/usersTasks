using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersTasks.Models;

namespace UsersTasks.Services
{
    public class TaskService
    {
        private readonly MyConfiguration configuration;
        protected UsersTasksContext _context;

        public TaskService(MyConfiguration configuration, UsersTasksContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        //
        // Summary:
        //     Finds and returns a task by it's Id field.
        //
        // Parameters:
        //   taskId:
        //     The Task Id key field.
        //
        // Returns:
        //     The task that was found.
        //
        public Models.Task GetTaskById(long taskId)
        {
            try
            {
                Models.Task task = _context.Tasks.Find(taskId);

                if (task == null)
                {
                    throw new Exception("Task not found");
                }

                return task;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds and returns all the tasks of a user after a join query on both UsersTasks and Tasks DbSets.
        //
        // Parameters:
        //   userId:
        //     The User's Id key field.
        //
        // Returns:
        //     All the tasks of the user.
        //
        public List<Models.Task> GetUserTasks(string userId)
        {
            try
            {
                long longUserId;
                if (!(long.TryParse(userId, out longUserId)))
                {
                    throw new Exception("Token not valid");
                }

                List<Models.Task> userTasks = _context.UsersTasks
                    .Where(userTask => userTask.UserId == longUserId)
                    .Join(
                    _context.Tasks,
                    userTask => userTask.TaskId,
                    task => task.TaskId,
                    (userTask, task) => task).ToList();

                return userTasks;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Checks if a task is linked to a given user.
        //
        // Parameters:
        //   userId:
        //     The User's Id key field.
        //
        //   taskId:
        //     The task's Id key field.
        //
        // Returns:
        //     True if a task is associated to the user was found.
        //
        public bool IsUserTaskExist(long userId, long taskId)
        {
            try
            {
                UserTask usrTask = _context.UsersTasks
                .Where(userTask => userTask.UserId == userId && userTask.TaskId == taskId)
                .FirstOrDefault();

                return usrTask != null;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Adds a task and associates it to a given user.
        //
        // Parameters:
        //   newTask:
        //     The task to add.
        //
        //   userId:
        //     The UserId to associates the task to.
        //
        // Returns:
        //     The given task.
        //
        public Models.Task AddUserTask(Models.Task newTask, string userId)
        {
            try
            {
                long longUserId;
                if (!(long.TryParse(userId, out longUserId)))
                {
                    throw new Exception("Token not valid");
                }

                _context.Tasks.Add(newTask);

                _context.SaveChanges();

                UserService userService = new UserService(configuration, _context);

                User user = userService.GetUserById(longUserId);

                UserTaskService userTaskService = new UserTaskService(configuration, _context);

                userTaskService.AssociateTaskToUser(newTask, user);

                return newTask;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Adds a list of tasks to the DbContext.
        //
        // Parameters:
        //   tasks:
        //     A list of tasks to add.
        //
        // Returns:
        //     True if everything was added fine.
        //
        public bool AddTasks(List<Models.Task> tasks)
        {
            try
            {
                _context.Tasks.AddRange(tasks);

                int tasksAdded = _context.SaveChanges();

                return tasksAdded > 0;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds a list of tasks and sends them to the associated user by Email.
        //
        // Parameters:
        //   userId:
        //     A UserId to find his tasks and send him a mail.
        //
        // Returns:
        //     True if everything was sent fine.
        //
        public bool SendTasksToUser(string userId)
        {
            try
            {
                long longUserId;
                if (!(long.TryParse(userId, out longUserId)))
                {
                    throw new Exception("Token not valid");
                }

                UserService userService = new UserService(configuration, _context);

                User user = userService.GetUserById(longUserId);

                List<Models.Task> userTasks = GetUserTasks(userId);

                if (userTasks.Count == 0)
                {
                    throw new Exception("No tasks to send");
                }

                MailService mailService = new MailService(configuration);

                string messageBodyPrefix = configuration.email.messageBodyPreprefix + " " + user.FirstName + configuration.email.messageYourTasksBodyPrefix;
                string messageBodySufix = configuration.email.messageSuffix + configuration.email.fromName;
                string messageBody = mailService.BuildMessageBodyFromTasks(userTasks, messageBodyPrefix, messageBodySufix);

                mailService.SendEmail(user.UserName, user.Email, configuration.email.subjectYourTasks, messageBody);

                return true;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds a list of tasks (their Ids are given) and if they are not already associated,
        //     associate them to a new user and then sends them to the new user.
        //
        // Parameters:
        //   shareData:
        //     A shareData object containing the list of tasks to share and the new userId to share with.
        //
        //   userId:
        //     A Id of the already associated user.
        //
        // Returns:
        //     True if everything was associated and sent fine.
        //
        public bool ShareTasksWithUser(TasksShareData shareData, string userId)
        {
            try
            {
                if (shareData.TasksIdsToShare.Count == 0)
                {
                    throw new Exception("No tasks to share");
                }

                long longUserId;
                if (!(long.TryParse(userId, out longUserId)))
                {
                    throw new Exception("Token not valid");
                }

                UserService userService = new UserService(configuration, _context);

                User sharingUser = userService.GetUserById(longUserId);
                List<Models.Task> sharingUserTasks = GetUserTasks(userId);

                User userToShare = userService.GetUserById(shareData.UserIdToShare);
                List<long> userToShareExistingTasksIds = GetUserTasks(shareData.UserIdToShare.ToString()).Select(tsk => tsk.TaskId).ToList(); ;

                List<long> newTasksIdsToShare = new List<long>();

                foreach (long TasksIdToShare in shareData.TasksIdsToShare)
                {
                    if (!userToShareExistingTasksIds.Contains(TasksIdToShare))
                    {
                        newTasksIdsToShare.Add(TasksIdToShare);
                    }
                }

                List<Models.Task> tasksToShare = new List<Models.Task>();

                foreach (Models.Task sharingUserTask in sharingUserTasks)
                {
                    if (newTasksIdsToShare.Contains(sharingUserTask.TaskId))
                    {
                        tasksToShare.Add(sharingUserTask);
                    }
                }

                if (tasksToShare.Count == 0)
                {
                    throw new Exception("No tasks to share");
                }

                UserTaskService userTaskService = new UserTaskService(configuration, _context);

                userTaskService.AssociateTasksToUser(tasksToShare, userToShare);

                MailService mailService = new MailService(configuration);

                string messageBodyPrefix = configuration.email.messageBodyPreprefix + " " + userToShare.FirstName + ",\n" + sharingUser.UserName + configuration.email.messageSharedTasksBodyPrefix;
                string messageBodySufix = configuration.email.messageSuffix + configuration.email.fromName;
                string messageBody = mailService.BuildMessageBodyFromTasks(tasksToShare, messageBodyPrefix, messageBodySufix);

                mailService.SendEmail(userToShare.UserName, userToShare.Email, sharingUser.UserName + " " + configuration.email.subjectSharedTasks, messageBody);

                return true;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds and modifies a task with new data.
        //
        // Parameters:
        //   task:
        //     The Task with the new data (and the task Id).
        //
        //   userId:
        //     The userId of the user associated with task to modify.
        //
        // Returns:
        //     The new modified task.
        //
        public Models.Task ModifyTask(Models.Task task, string userId)
        {
            try
            {
                long longUserId;
                if (!(long.TryParse(userId, out longUserId)))
                {
                    throw new Exception("Token not valid");
                }

                if (!IsUserTaskExist(longUserId, task.TaskId))
                {
                    throw new Exception("User is not permitted to modify task");
                }

                _context.Entry(task).State = EntityState.Modified;
                _context.SaveChanges();

                return task;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
