using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.IO;
using SAILIIOS;

namespace SAILIIOS1
{
    public partial class PasswordResetController : UIViewController
    {
        private string pathToDatabase;
        public User enteredUser;
        public PasswordResetController (IntPtr handle) : base (handle)
        {
            
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			pathToDatabase = Path.Combine(documentsFolder, "SAILIUser_db.db");

        }

        partial void ChangePasswordButton_TouchUpInside(UIButton sender)
        {
            
            if(NewPasswordText.Text.Equals(ConfirmPwdText.Text))
            {
                if(SignUpPageController.CheckPassword(NewPasswordText.Text))
                {

                    /*
                     *  Update and Save password to the datebase
                     */
                    PasswordManager PM = new PasswordManager();

                    using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
                    {
                        var query = connection.Get<User>(enteredUser.Username);

                        query.HashedPassword = PM.GeneratePasswordHash(NewPasswordText.Text, enteredUser.OwnerSalt);
                        connection.Update(query);
                    }

                    NavigationController.PopToRootViewController(true);
                    popAlert("Successful", "Password Updated Successfully","OK");
                }
                else 
                {
                   popAlert("Password format Incorrect", "Password should contain number , upper and lower case","OK");
                }
            }
            else
            {
                popAlert("Password not match", "Please make sure the newpassword and confirmpasswrod is the same","OK");
            }
        }


		

		public void popAlert(string alertTitle, string alertInfo, string alertbutton)
		{
			var alert = UIAlertController.Create(alertTitle, alertInfo, UIAlertControllerStyle.Alert);
			alert.AddAction(UIAlertAction.Create(alertbutton, UIAlertActionStyle.Default, null));
			PresentViewController(alert, true, null);
		}
    }
}