using System;
using UIKit;
using System.Collections.Generic;
using System.IO;



namespace SAILIIOS1
{
    public partial class AccountVerificationController : UIViewController
    {
        private string pathToData;
        private List<User> Users;
        public AccountVerificationController (IntPtr handle) : base (handle)
        {
            Users = new List<User>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            pathToData = Path.Combine(documentsFolder, "SAILIUser_db.db");

        }

        partial void VerifyButton_TouchUpInside(UIButton sender)
        {
            using (var connection = new SQLite.SQLiteConnection(pathToData))
            {
                var query = connection.Table<User>();
                foreach (User user in query)
                {
                    Users.Add(user);
                }

                if (gothroughDatabaseByUsername(Users , UsernameText.Text) != null)
                {
                    User wanttedUser = gothroughDatabaseByUsername(Users, UsernameText.Text);
                    if(wanttedUser.Email.Equals(EmailText.Text))
                    {
						PasswordResetController PRC = this.Storyboard.InstantiateViewController("prc") as PasswordResetController;
                        PRC.enteredUser = wanttedUser;
						NavigationController.PushViewController(PRC, true);
					}
                    else
                    {
						var alert = UIAlertController.Create("Email not match", "Please Input correct email of your account", UIAlertControllerStyle.Alert);
						alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
						PresentViewController(alert, true, null);
					}
                }
				else
				{
				        var alert = UIAlertController.Create("Username does not exist", "Please Input correct Username of your account", UIAlertControllerStyle.Alert);
				        alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
				        PresentViewController(alert, true, null);
				}
			}
        }

		public static User gothroughDatabaseByUsername(List<User> list, string wanttedname)
		{
			User wanttedUser = new User();
			foreach (User user in list)
			{
				if (user.Username.Equals(wanttedname))
				{
					wanttedUser = user;
				}

			}
			return wanttedUser;
		}

       
    }

}