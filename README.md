ciapi-oauth2-service
====================

A Trading API specific OAuth2 ecosystem comprised of 

1. an Authentication Server (AS) that manages correlation of access tokens<->api sessions
2. a token handler module for the Trading API (RS) that transparently converts OAuth2 authenicated requests to username/session requests
3. client (RP) reference implementations

## Architecture Overview

![Architecture Overview](https://f.cloud.github.com/assets/117368/887061/5e37a8d4-f9f5-11e2-8c65-c5b96a501822.png)
