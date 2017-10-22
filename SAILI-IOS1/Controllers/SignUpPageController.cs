using Foundation;
using System;
using UIKit;
using System.IO;
using SQLite;
using SAILIIOS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace SAILIIOS1
{
    public partial class SignUpPageController : UIViewController
    {
        private string pathToDatabase;
        private List<User> Users;

        public SignUpPageController (IntPtr handle) : base (handle)
        {
            Users = new List<User>();
        }


        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            pathToDatabase = Path.Combine(documentsFolder, "SAILIUser_db.db");

            using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
            {
                connection.CreateTable<User>();
            }


        }

        partial void SignUpButton_TouchUpInside(UIButton sender)
        {

            using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
            {
                
                /*
                check if password match
                */


                //if password match
                if((PasswordText.Text).Equals(ConfirmPasswordText.Text))
                {
                    //check if entered Username existed
                    if(!isAccountExisted(UsernameText.Text,pathToDatabase))
                    {
                        /*
                         * Check if User type valid
                         */


                        //Username should more than 5 charactors
                        if (UsernameText.Text.Length > 5)
                        {

                            //Password should contain number and upper & lower case
                            if(CheckPassword(PasswordText.Text))
                            {

                                //Email should in the correct format
                                if (CheckEmailValidation(EmailText.Text))
                                {
                                    

                                    //  Creaet Newuser and Save into database
                                    User newuser = generateUser(UsernameText.Text, PasswordText.Text, EmailText.Text);
                                    connection.Insert(newuser);


                                    //After login successfully , auto go to the userpage controller

                                    popToViewController("userpagecontroller", UsernameText.Text);
									
                                }
                                else
                                {
                                    // If Email format not correct , show Alert

                                    popAlert("Email format not correct", "Please type a valid email address", "OK");
                                }

                            }
                            else
                            {
								// If Password format not correct , show Alert
								
                                popAlert("Password format incorrect", "Password should contain number , upper and lower case","OK");
                            }
                        }
                        else
                        {
                            // If Username format not correct , show Alert

                            popAlert("Username format incorrect", "Username should have more than 5 charactor", "OK");
                        }
					}
                    /*
                     * if User existed , show alert
                     */
                    else
                    {
                        popAlert("Username already used", "Please use a new one","OK");
                    }
					
                }

                // If password not match , show alert
                else
                {
                    popAlert("Password not match", "Please make sure ConfirmPassword match the password","OK");
                }

            }
        }






        public Boolean isAccountExisted(string Username,string databaselocation)
        {
            Boolean exist = false;
            using (var connection = new SQLite.SQLiteConnection(databaselocation))
            {
                var query = connection.Table<User>();
                foreach(User user in query)
                {
                    if(user.Username.Equals(Username))
                    {
                        exist = true;
                    }
                }
            }
            return exist;
        }

        //check Email Validation
        public Boolean CheckEmailValidation(String email)
        {
            bool EmailisValid = false; 
            try
            {
                var mailAddress = new MailAddress(email);
                EmailisValid = true;
                return EmailisValid;
            }
            catch(FormatException)
            {
                return EmailisValid;
            }
            
        }

        //Check if password follow the rule
        public static bool CheckPassword(string password)
        {
            bool PasswordValid = false;
            string pattern = "(?=.*[A-Z])(?=.*[0-9])(?=.*[a-z])";
            Match match = Regex.Match(password, pattern);
            if(match.Success)
            {
                PasswordValid = true;
            }
            return PasswordValid;
        }

        public User generateUser(string username, string password, string email)
        {
            User newuser = new User();
			/*
             * 
             * Hash and salt the password
             * 
             */
			PasswordManager PM = new PasswordManager();
			string salt = SaltGenerator.GetSaltString();
            string HashedPwd = PM.GeneratePasswordHash(password, salt);
			newuser.Username = username;
			newuser.HashedPassword = HashedPwd;
			newuser.OwnerSalt = salt;
            newuser.Email = email;

            return newuser;
        }

        public  void popToViewController (string ViewControllerName , string LoginedUsername)
        {
            UserPageController UPC = this.Storyboard.InstantiateViewController(ViewControllerName) as UserPageController;
			NavigationController.PushViewController(UPC, true);
            UPC.inAccountName = "Welcome" + LoginedUsername;
            popAlert("Successful" , "You have login successfully","OK");
        }

        public  void popAlert (string alertTitle, string alertInfo, string alertbutton)
        {
            var alert = UIAlertController.Create(alertTitle, alertInfo, UIAlertControllerStyle.Alert);
            alert.AddAction(UIAlertAction.Create(alertbutton, UIAlertActionStyle.Default, null));
			PresentViewController(alert, true, null);
        }
		
    }
    public class User 
    {
        [Required]
        [PrimaryKey]
        public string Username { set; get; }

        [Required]
        public string HashedPassword { set; get; }

        [Required]
        public string OwnerSalt { set; get; }

        [Required]
        public string Email { set; get; }

        public User()
        {
        }
    }
}