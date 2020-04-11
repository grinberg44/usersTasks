using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersTasks.Models;

namespace UsersTasks.Services
{
    public class UserService
    {
        private readonly MyConfiguration configuration;
        protected UsersTasksContext _context;

        public UserService(MyConfiguration configuration, UsersTasksContext context)
        {
            this.configuration = configuration;
            _context = context;
        }

        //
        // Summary:
        //     Returns all the users in the DbContext.
        //
        // Returns:
        //     A list of all the users.
        //
        public List<User> GetAllUsers()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Registers a new user with all it's data and returns a valid new token.
        //
        // Parameters:
        //   User:
        //     A new user with all it's data.
        //
        // Returns:
        //     A string representing a valid new token.
        //
        public string RegisterUserAndGetToken(User user)
        {
            try
            {
                User existingUser = _context.Users
                .Where(usr => usr.UserName == user.UserName || usr.Email == user.Email || usr.PhoneNumber == user.PhoneNumber)
                .FirstOrDefault();

                if (existingUser != null)
                {
                    throw new Exception("UserName, email or phone number already taken");
                }

                AuthService auth = new AuthService(configuration);

                user.Role = "standartUser";

                user.Password = auth.HashPassword(user.Password);

                _context.Users.Add(user);

                _context.SaveChanges();

                if (user.Tasks != null && user.Tasks.Count > 0)
                {
                    TaskService taskService = new TaskService(configuration, _context);

                    taskService.AddTasks(user.Tasks);

                    UserTaskService userTaskService = new UserTaskService(configuration, _context);

                    userTaskService.AssociateTasksToUser(user.Tasks, user);
                }

                string token = auth.CreateToken(user);

                return token;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Checks if a user is valid and if so returns a valid new token.
        //
        // Parameters:
        //   loginUser:
        //     A loginUser data containing the userName and password.
        //
        // Returns:
        //     A string representing a valid new token.
        //
        public string LoginUserAndGetToken(LoginUser loginUser)
        {
            try
            {
                User user = _context.Users
                .Where(usr => usr.UserName == loginUser.UserName)
                .FirstOrDefault();

                if (user == null)
                {
                    throw new Exception("UserName does not exist");
                }

                AuthService auth = new AuthService(configuration);

                if (!auth.ValidatePassword(loginUser.Password, user.Password))
                {
                    throw new Exception("Password is not correct");
                }

                string token = auth.CreateToken(user);

                return token;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds and returns a user by it's Email address.
        //
        // Parameters:
        //   taskId:
        //     The Email address of the user.
        //
        // Returns:
        //     The user that was found.
        //
        public User GetUserByEmail(string email)
        {
            try
            {
                User user = _context.Users.SingleOrDefault(usr => usr.Email == email);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return user;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Finds and returns a user by it's Id field.
        //
        // Parameters:
        //   userId:
        //     The user Id key field.
        //
        // Returns:
        //     The user that was found.
        //
        public User GetUserById(long userId)
        {
            try
            {
                User user = _context.Users.Find(userId);

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                return user;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
