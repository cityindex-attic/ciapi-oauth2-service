ciapi-oauth2-service
====================

An extension service for the CIAPI that implements an OAuth2 like flow for getting a new SessionId.

## Website flow

![Flow diagram](https://f.cloud.github.com/assets/227505/98045/e4cbf224-6710-11e2-9b83-90764ca02693.jpg)

1.  Your app redirects browser to `authServer + "?returnUrl=" + UrlEncode(ReturnUrl)`
2.  User enters their login details into the authServer, 
3.  which creates CIAPI `Session`
3.  authServer redirects browser back to `ReturnUrl + '?auth={encryptedCode}'`
4.  Your app makes server side request to `authServer + "/api/Decrypt/" + HttpUtility.UrlEncode({encryptedCode})`
5.  authServer returns `Username:Session`
6.  Your app can now make CIAPI calls using `Username` and `Session` headers as normal
