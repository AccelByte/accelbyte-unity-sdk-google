// Copyright (c) 2025 AccelByte Inc. All Rights Reserved.
// This is licensed software from AccelByte Inc, for limitations
// and restrictions contact your company contract manager.

using UnityEngine;
using UnityEngine.UI;

namespace AccelByte.Google.Samples.SignInWithGoogle
{
    public class LoginController : MonoBehaviour
    {
        public LoginHandler LoginHandler;
        public Button LoginButton;
        public Text LoginStatus;
        public Button LogoutButton;

        private LoginHandler.LoginResultType lastLoginResultType;

        private void Start()
        {
            LoginHandler = new LoginHandler();
            
            LoginButton?.onClick.AddListener(IntegratedLogin);
            LogoutButton?.onClick.AddListener(Logout);
            UpdateUI();
            LoggedOutState();
        }

        private void IntegratedLogin()
        {
            LoginButton.enabled = false;
            var loginEnumerator = LoginHandler.RunAutoLogin((loginResult) =>
            {
                LoginButton.enabled = true;
                
                lastLoginResultType = loginResult.ResultType;
                UpdateUI();
                if (!string.IsNullOrEmpty(loginResult.Log) && LoginStatus != null)
                {
                    LoginStatus.text += $"\n{loginResult.Log}";
                }
                
                LoggedInState();
            });
            StartCoroutine(loginEnumerator);
        }

        private void Logout()
        {
            LoginHandler.AGSLogout(() =>
            {
                lastLoginResultType = LoginHandler.GetCurrentLoginResult();
                UpdateUI();
                LoggedOutState();
            });
        }

        private void UpdateUI()
        {
            string message = "Login Status: ";
            switch (lastLoginResultType)
            {
                case LoginHandler.LoginResultType.Failed:
                    message += "Login Failed";
                    break;
                case LoginHandler.LoginResultType.Success:
                    message += "Login Success";
                    break;
                case LoginHandler.LoginResultType.None:
                default:
                    message += "User not signed in";
                    break;
            }
            if (LoginStatus != null)
            {
                LoginStatus.text = message;
            }
        }

        private void LoggedInState()
        {
            if (lastLoginResultType == LoginHandler.LoginResultType.Success)
            {
                LoginButton?.gameObject.SetActive(false);
                LogoutButton?.gameObject.SetActive(true);
            }
            else
            {
                LoginButton?.gameObject.SetActive(true);
                LogoutButton?.gameObject.SetActive(false);
            }
        }

        private void LoggedOutState()
        {
            LoginButton?.gameObject.SetActive(true);
            LogoutButton?.gameObject.SetActive(false);
        }
    }
}
