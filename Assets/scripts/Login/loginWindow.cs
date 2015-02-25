using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class loginWindow : MonoBehaviour {

	public Text prompt;

	private string loginURL = "http://www.stuffcluster.com/social/AdvancedLogin/login.php";
	private string registerURL = "http://www.stuffcluster.com/social/AdvancedLogin/register.php";

	private string email = "";
	private string password = "";
	private string passwordRepeat = "";
	private string firstname = "";
	private string lastname = "";

	public InputField emailInput;
	public InputField passwordInput;
	public InputField passwordRepeatInput;
	public InputField firstnameInput;
	public InputField lastnameInput;

	void Start(){
		//InvokeRepeating("checkConnection", 0, 20.0F);
	}

	public void GetFields(){
		//retrieve the fields from the inputs
		email = emailInput.text.text;
		password = passwordInput.text.text;
		passwordRepeat = passwordRepeatInput.text.text;
		firstname = firstnameInput.text.text;
		lastname = lastnameInput.text.text;
	}

	public void checkConnection(){
		StartCoroutine(testConnection());
	}

	public void tryLogin(){
		StartCoroutine(handleLogin(email, password));
	}

	public void tryRegister(){
		StartCoroutine(handleRegister(email, password, firstname, lastname));
	}

	IEnumerator handleLogin(string e, string pass){
		prompt.text = "Checking email and password";
		WWWForm form = new WWWForm();
		form.AddField("email", e);
		form.AddField("password", pass);

		WWW loginReader = new WWW(loginURL, form);
		yield return loginReader;

		if(loginReader.error != null){
			prompt.text = "Could not locate page";
		}else{
			string[] data = DivideString(loginReader.text);
			if(data[0] == "right"){
				prompt.text = "Log in successful.";
				saveLogin(data[1], data[2]);
				print (data[1]);
				print (data[2]);
				Application.LoadLevel("ThingFeedV5");
			}else{
				prompt.text = "invalid email/password";
				print (loginReader.text);
			}
		}
	}

	IEnumerator handleRegister(string e, string pass, string first, string sur){
		print ("registering");
		prompt.text = "Checking email and password";
		WWWForm form = new WWWForm();
		form.AddField("email", e);
		form.AddField("password", pass);
		form.AddField("firstname", first);
		form.AddField("surname", sur);
		
		WWW loginReader = new WWW(registerURL, form);
		yield return loginReader;
		
		if(loginReader.error != null){
			prompt.text = "Could not locate page";
		}else{
			if(loginReader.text == "registered"){
				prompt.text = "Register successful.";
				print ("Register successful.");
				StartCoroutine(handleLogin(e, pass));
			}else{
				prompt.text = "ooooops something breaked: ";
				print (loginReader.text);
			}
		}
	}

	IEnumerator testConnection(){
		prompt.text = "Checking connection to Systech server...";
		// send a wrong login just to test the connection
		WWWForm form = new WWWForm();
		form.AddField("email", "empty");
		form.AddField("password", "empty");
		
		WWW loginReader = new WWW(loginURL, form);
		yield return loginReader;
		
		if(loginReader.error != null){
			prompt.text = "Failed to connect to server";
		}else{
			prompt.text = "Connected to server";
		}
	}

	private void saveLogin(string user, string pass){
		PlayerPrefs.SetString("userCookie", user);
		PlayerPrefs.SetString("passCookie", pass);
	}

	private string[] DivideString(string intput){
		string[] attStringSeparators = {"<!>"};
		return intput.Split(attStringSeparators, System.StringSplitOptions.None);
	}
}
