using System;
using System.Collections.Generic;
using System.IO;
using UIKit;
using SAILIIOS;

namespace SAILIIOS1
{
    public partial class ViewController : UIViewController
    {
        private string pathToDatabase;
        private List<User> UserList;
        protected ViewController(IntPtr handle) : base(handle)
        {
            UserList = new List<User>();
        }
    			

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			pathToDatabase = Path.Combine(documentsFolder, "SAILIUser_db.db");

            /*
             *  Create User table if User table does not exist
             */
			using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
			{
				connection.CreateTable<User>();
			}


        }

        partial void LoginButton_TouchUpInside(UIButton sender)
        {
            
            PasswordManager PM = new PasswordManager();


			/*
             *  Read User table from database
             */
			using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
			{
				var query = connection.Table<User>();
				foreach (User user in query)
				{
					UserList.Add(user);
				}
			}

                //check if user existed in the database


                if(gothroughDatabaseByUsername(UserList,UsernameText.Text) != null)
                    
                    {
                    User selectedUser = gothroughDatabaseByUsername(UserList, UsernameText.Text);
                        //check if user typed match database account 

                    if (PM.IsPasswordMatch(PasswordText.Text, selectedUser.OwnerSalt, selectedUser.HashedPassword))
                        {
                        
						//After login successfully , auto go to the userpage controller

						popToViewController("userpagecontroller", UsernameText.Text);

                        }

                        //if account existed , password not match
                        else
                        {
                        
                          popAlert("Password Incorrect ", "Please input correct password","OK");

                        }
                    }

                    //if Account not existed
                    else
                    {
                    
                    popAlert("Username does not exist", "Please input correct Username and password to login","OK");

                    }
            
        }

        public static User gothroughDatabaseByUsername(List<User>list , string wanttedname)
        {
            User wanttedUser = new User();
            foreach(User user in list)
            {
                if (user.Username.Equals(wanttedname)) 
                {
                  wanttedUser = user;  
                }

            }
            return wanttedUser;
        }

		public void popToViewController(string ViewControllerName, string LoginedUsername)
		{
			UserPageController UPC = this.Storyboard.InstantiateViewController(ViewControllerName) as UserPageController;
			NavigationController.PushViewController(UPC, true);
			UPC.inAccountName = "Welcome" + LoginedUsername;
			popAlert("Successful", "You have login successfully", "OK");
		}

		public void popAlert(string alertTitle, string alertInfo, string alertbutton)
		{
			var alert = UIAlertController.Create(alertTitle, alertInfo, UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create(alertbutton, UIAlertActionStyle.Default, null));
			PresentViewController(alert, true, null);
		}

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}
