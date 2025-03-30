using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]

public class LoginData
{
    public string username;
    public string password;
}

// Class to represent the login response from the server
[System.Serializable]

/*{"status":"success","data":{"userid":"1","username":"testuser","email":"test@mail.com","diamond_count":"71"}}*/
public class LoginResponse
{
    public string status;        // "success" or "error"
    public string message;       // Message in case of error (e.g., "Invalid credentials")
    public Data data;     // The data part which contains user information
    public int new_diamond_count; //for update api use,temp value
}

[System.Serializable]
public class Data
{
    public int userid;        // User ID as string
    public string username;      // Username of the user
    public string email;         // User email
    public int diamond_count; // Diamond count as string (can be parsed to int later)
    
}

public enum WINDOWMODE
{
    LOGIN = 0,
    GAMEPLAY
};

public class MainUIScript : MonoBehaviour
{
    private string APIBASEURL = "https://akitacode.com/demo/";
    private string APILOGIN = "login_api.php";
    private string APIUSEDIAMOND = "use_diamonds.php";

    [Header("Login window")] 
    public Image win_login;
    public TMP_InputField ip_username, ip_password;
    public TMP_Text t_error_login;
    
    [Header("Gameplay window")] 
    public Image win_gameplay;
    public TMP_Text t_username, t_diamondcount, t_error;

    [Header("Loading window")]
    public Image win_loading;
    
    //inner use
    private LoginResponse currentPlayer;

    #region Start&Update

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        showUIMode(WINDOWMODE.LOGIN);
        ip_username.onEndEdit.AddListener((text) =>
        {
            nextFieldAction(ip_username);
        });
        ip_password.onEndEdit.AddListener((text) =>
        {
            nextFieldAction(ip_password);
        });
    }

    // Update is called once per frame
    void Update()
    {
        TMP_InputField selectedInputField = EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_InputField>();
        if (selectedInputField != null)
        {
                checkInputField(selectedInputField);
        }
    }

    #endregion

    #region UI

    public void showUIMode(WINDOWMODE _mode)
    {
        win_loading.gameObject.SetActive(false);
        win_login.gameObject.SetActive(_mode == WINDOWMODE.LOGIN);
        win_gameplay.gameObject.SetActive(_mode == WINDOWMODE.GAMEPLAY);
        switch (_mode)
        {
            case WINDOWMODE.LOGIN:
                if (PlayerPrefs.HasKey("userid"))
                {
                    ip_username.text = PlayerPrefs.GetString("userid");
                    ip_password.text = "";
                    ip_password.Select();
                }
                else
                {
                    ip_username.Select();
                }
                
                break;
        }
    }

    #endregion

    #region Login window action

    public void tab_login()
    {
        string username = ip_username.text;
        string password = ip_password.text;

        // Validate that both fields are filled in
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            t_error_login.text = "Username or password is empty.";
            return;
        }

        // Create the login data (the data to send to the PHP API)
        LoginData loginData = new LoginData
        {
            username = username,
            password = password
        };

        //show loading/prevent action
        win_loading.gameObject.SetActive(true);
        // Start the coroutine to send the login request
        StartCoroutine(LoginRequest(loginData));
    }
    
    private IEnumerator LoginRequest(LoginData loginData)
    {
        // Convert loginData into JSON format
        string json = JsonUtility.ToJson(loginData);

        // Create a UnityWebRequest to send the data to the login API
        using (UnityWebRequest request = new UnityWebRequest(APIBASEURL+APILOGIN, "POST"))
        {
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Wait for the response
            yield return request.SendWebRequest();

            //Hide loader
            win_loading.gameObject.SetActive(false);
            
            // Check for errors in the request
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("Login request failed: " + request.error);
                t_error_login.text = request.error;
            }
            else
            {
                // Parse the response from the API
                string responseText = request.downloadHandler.text;
                Debug.Log("Response from server: " + responseText);

                // Handle the response (assuming JSON response from the server)
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);
                
                if (response.status == "success")
                {
                    Debug.Log("Login successful!");
                    Debug.Log("User ID: " + response.data.userid);
                    Debug.Log("Diamond Count: " + response.data.diamond_count);
                    //show gameplay windows
                    showUIMode(WINDOWMODE.GAMEPLAY);
                    //setup initial player data UI
                    t_username.text = response.data.username;
                    t_diamondcount.text = "x "+response.data.diamond_count.ToString();
                    t_error_login.text = "";
                    currentPlayer = response;
                    saveUserID(response.data.userid, response.data.username);
                }
                else
                {
                    Debug.LogError("Login failed: " + response.message);
                    // Show an error message
                    t_error_login.text = response.message;
                }
            }
        }
    }

    //save to cache
    void saveUserID(int _userid, string _username)
    {
        PlayerPrefs.SetInt("userid", _userid);
        PlayerPrefs.SetString("username", _username);
        PlayerPrefs.Save();
    }

    #endregion
    
    #region Gameplay window action

    public void tab_usediamond()
    {
        // Check if the player is logged in by verifying if the user ID is saved in PlayerPrefs
        if (currentPlayer != null)
        {
            int userId = currentPlayer.data.userid;
            StartCoroutine(SendUseDiamondsRequest(userId, 5));
        }
        else
        {
            Debug.LogError("User is not logged in.");
        }
    }    
    
    public void tab_logout()
    {
        //clear data
        currentPlayer = null;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        showUIMode(WINDOWMODE.LOGIN);
    }
    
    // Coroutine to handle the POST request
    private IEnumerator SendUseDiamondsRequest(int userid, int diamondsUsed)
    {
        // Create a JSON object to send in the request
        string jsonData = "{\"userid\": " + userid + ", \"diamonds_used\": " + diamondsUsed + "}";

        // Convert JSON data to byte array
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create UnityWebRequest with POST method and send JSON data
        UnityWebRequest request = new UnityWebRequest(APIBASEURL + APIUSEDIAMOND, "POST");
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for response
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the JSON response
            string responseText = request.downloadHandler.text;
            t_error.text = "";
            HandleResponse(responseText);
        }
        else
        {
            // Handle error response
            Debug.LogError("Error: " + request.error);
            t_error.text = request.error;
        }
    }

    // Handle the response from the API
    private void HandleResponse(string responseText)
    {
        // Parse the JSON response
        LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);
        
        if (response.status == "success")
        {
            Debug.Log("Diamonds used successfully!");
            Debug.Log("Remaining Diamonds: " + response.new_diamond_count);
            currentPlayer.data.diamond_count = response.new_diamond_count;
            t_diamondcount.text = "x " + currentPlayer.data.diamond_count.ToString();
            t_error.text = "";
        }
        else
        {
            Debug.LogError("Error: " + response.message);
            t_error.text = response.message;
        }
    }

    #endregion

    #region Tab/Enter press

    private void checkInputField(TMP_InputField _field)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Tab))
        {
            //for quick mockup, we manually select next action here
            /**ip_username, ip_password*/
            nextFieldAction(_field);
        }
    }

    void nextFieldAction(TMP_InputField _field)
    {
        if (_field == ip_username)
        {
            ip_password.Select(); 
            ip_password.ActivateInputField();
        }
        else if (_field == ip_password)
        {
            //do login action
            tab_login();
        }
    }

    #endregion
}
