using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersTasks.Models;

namespace UsersTasks.Services
{
    public class UserTaskService
    {
        private readonly MyConfiguration configuration;
        protected UsersTasksContext _context;

        public UserTaskService(MyConfiguration configuration, UsersTasksContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        //
        // Summary:
        //     Associates a task to a user.
        //
        // Parameters:
        //   task:
        //     The Task to associate.
        //
        //   user:
        //     The user to associate the task to.
        //
        // Returns:
        //     True if everything was fine.
        //
        public bool AssociateTaskToUser(Models.Task task, User user)
        {
            try
            {
                UserTask userTask = new UserTask(user.UserId, task.TaskId);

                _context.UsersTasks.Add(userTask);

                _context.SaveChanges();

                return true;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Associates a list of tasks to a user.
        //
        // Parameters:
        //   tasks:
        //     The list of tasks to associate.
        //
        //   user:
        //     The user to associate the tasks to.
        //
        // Returns:
        //     True if everything was fine.
        //
        public bool AssociateTasksToUser(List<Models.Task> tasks, User user)
        {
            try
            {

                List<UserTask> userTasks = new List<UserTask>();

                foreach (Models.Task task in tasks)
                {
                    UserTask userTask = new UserTask(user.UserId, task.TaskId);

                    userTasks.Add(userTask);
                }

                _context.UsersTasks.AddRange(userTasks);

                int usersTasksAdded = _context.SaveChanges();

                return usersTasksAdded > 0;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
