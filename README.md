# ciapi-oauth2-service

An extension service for the CIAPI that implements an OAuth2 like flow for getting a new SessionId.

## Status

![Incomplete](http://labs.cityindex.com/wp-content/uploads/2012/01/lbl-incomplete.png)![Unsupported](http://labs.cityindex.com/wp-content/uploads/2012/01/lbl-unsupported.png)

This project has been retired and is no longer being supported by City Index Ltd.

* if you should choose to fork it outside of City Index, please let us know so we can link to your project

## Website flow

![Flow diagram](https://f.cloud.github.com/assets/227505/98045/e4cbf224-6710-11e2-9b83-90764ca02693.jpg)

1.  Your app redirects browser to `authServer + "?returnUrl=" + UrlEncode(ReturnUrl)`
2.  User enters their login details into the authServer, 
3.  which creates CIAPI `Session`
3.  authServer redirects browser back to `ReturnUrl + '?auth={encryptedCode}'`
4.  Your app makes server side request to `authServer + "/api/Decrypt/" + HttpUtility.UrlEncode({encryptedCode})`
5.  authServer returns `"Username:Session"`
6.  Your app removes `"` and splits on : to retrieve value `Username` and `Session` values.  
7.  Now your app can make calls directly to CIAPI using `Username` and `Session` values in your request header, as per normal

## License

Copyright 2013 City Index Ltd.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

  [http://www.apache.org/licenses/LICENSE-2.0](http://www.apache.org/licenses/LICENSE-2.0)

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
