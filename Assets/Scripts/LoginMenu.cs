

using UnityEngine;
using System; 
using System.Collections;
using DatabaseControl;

public class LoginMenu : MonoBehaviour {
	
	

	public GameObject login_object;
	public GameObject register_object;
	public GameObject loading_object;

	public UnityEngine.UI.InputField input_login_username;
	public UnityEngine.UI.InputField input_login_password;
	

	public UnityEngine.UI.InputField input_register_username;
	public UnityEngine.UI.InputField input_register_password;
	public UnityEngine.UI.InputField input_register_confirmPassword;
	

	public UnityEngine.UI.Text login_error;
	public UnityEngine.UI.Text register_error;
	

	

	int part = 0;


	bool isDatabaseSetup = true;

	void Start () {


		TextAsset datafile = Resources.Load ("data") as TextAsset;
		string[] splitdatafile = datafile.text.Split (new string[] { "-" }, StringSplitOptions.None);
		if (splitdatafile [0] == "0") {
			isDatabaseSetup = false;
			Debug.Log ("These demos will not work out of the box. You need to setup a database first for it to work. Please read the Setup section of the PDF for more information");
		} else {
			isDatabaseSetup = true;
		}


		blankErrors();
	}

	void Update () {

		if (isDatabaseSetup == true) {


			if (part == 0) {
				login_object.gameObject.SetActive (true);
				register_object.gameObject.SetActive (false);
				loading_object.gameObject.SetActive (false);
			}
			if (part == 1) {
				login_object.gameObject.SetActive (false);
				register_object.gameObject.SetActive (true);
				loading_object.gameObject.SetActive (false);
			}
			if (part == 2) {
				
			}
			if (part == 3) {
				login_object.gameObject.SetActive (false);
				register_object.gameObject.SetActive (false);
				loading_object.gameObject.SetActive (true);
			}

		}
		
	}

	void blankErrors () {
		
		login_error.text = "";
		register_error.text = "";
	}
	
	public void login_Register_Button () {
		part = 1; 
		blankErrors();
	}
	
	public void register_Back_Button () { 
		part = 0;
		blankErrors();
	}
	
	public void data_LogOut_Button () { 
		part = 0;

		UserAccountManager.instance.LogOut();

		blankErrors();
	}

	public void login_login_Button () { 
		
		if (isDatabaseSetup == true) {
		

			if ((input_login_username.text != "") && (input_login_password.text != "")) {
			

				if ((input_login_username.text.Contains ("-")) || (input_login_password.text.Contains ("-"))) {
					
					login_error.text = "Unsupported Symbol '-'";
					input_login_password.text = "";
				} else {

					StartCoroutine (sendLoginRequest (input_login_username.text, input_login_password.text));
					part = 3;
				}
			
			} else {
				
				login_error.text = "Field Blank!";
				input_login_password.text = "";
			}
		
		}
		
	}
	
	IEnumerator sendLoginRequest (string username, string password) {

		if (isDatabaseSetup == true) {
		
			IEnumerator e = DC.Login (username, password);
			while (e.MoveNext()) {
				yield return e.Current;
			}
			WWW returned = e.Current as WWW;
			if (returned.text == "Success") {
				
				blankErrors ();
				part = 2; 
			

				input_login_username.text = "";

				UserAccountManager.instance.LogIn(username, password);
			}
			if (returned.text == "incorrectUser") {
				
				login_error.text = "Username not found";
				part = 0; 
			}
			if (returned.text == "incorrectPass") {
				
				part = 0; 
				login_error.text = "Incorrect Password";
			}
			if (returned.text == "ContainsUnsupportedSymbol") {
				
				part = 0; 
				login_error.text = "Unsupported Symbol '-'";
			}
			if (returned.text == "Error") {
				
				part = 0; 
				login_error.text = "Database Error. Try again later.";
			}
		

			input_login_password.text = "";

		}
	}

	public void register_register_Button () {

		if (isDatabaseSetup == true) {
		

			if ((input_register_username.text != "") && (input_register_password.text != "") && (input_register_confirmPassword.text != "")) {
			

				if (input_register_username.text.Length > 4) {
				

					if (input_register_password.text.Length > 6) {
					

						if (input_register_password.text == input_register_confirmPassword.text) {
						
							if ((input_register_username.text.Contains ("-")) || (input_register_password.text.Contains ("-")) || (input_register_confirmPassword.text.Contains ("-"))) {
							

								register_error.text = "Unsupported Symbol '-'";
								input_login_password.text = "";
								input_register_confirmPassword.text = "";
							
							} else {
							

								StartCoroutine (sendRegisterRequest (input_register_username.text, input_register_password.text, "[KILLS]0/[DEATHS]0")); //calls function to send register request
								part = 3; 
							}
						
						} else {
							
							register_error.text = "Passwords don't match!";
							input_register_password.text = ""; 
							input_register_confirmPassword.text = "";
						}
					
					} else {
						
						register_error.text = "Password too Short";
						input_register_password.text = ""; 
						input_register_confirmPassword.text = "";
					}
				
				} else {
					
					register_error.text = "Username too Short";
					input_register_password.text = "";
					input_register_confirmPassword.text = "";
				}
			
			} else {
				
				register_error.text = "Field Blank!";
				input_register_password.text = ""; 
				input_register_confirmPassword.text = "";
			}

		}
		
	}
	
	IEnumerator sendRegisterRequest (string username, string password, string data) {

		if (isDatabaseSetup == true) {
		
			IEnumerator ee = DC.RegisterUser(username, password, data);
			while(ee.MoveNext()) {
				yield return ee.Current;
			}
			WWW returnedd = ee.Current as WWW;
			
			if (returnedd.text == "Success") {
				
				blankErrors();
				part = 2; 
				

				input_register_username.text = ""; 

				UserAccountManager.instance.LogIn(username, password);
			}
			if (returnedd.text == "usernameInUse") {
				
				part = 1;
				register_error.text = "Username Unavailable. Try another.";
			}
			if (returnedd.text == "ContainsUnsupportedSymbol") {
				
				part = 1;
				register_error.text = "Unsupported Symbol '-'";
			}
			if (returnedd.text == "Error") {
				
				part = 1;
				login_error.text = "Database Error. Try again later.";
			}
			
			input_register_password.text = "";
			input_register_confirmPassword.text = "";

		}
	}

}
