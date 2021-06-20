# Link Shortener API
Аn aptitude test for the position of C# .Net backend developer.

REST API of the link shortening service.

API capabilities:
- creating a short link based on the original link
- receiving the original via a shortened link, with an increase in the counter of visits
- getting a list of all shortened links with the number of link followings

# How to use
## Registration
1. Register a new user by sending json with the following structure
    ```
    {"email": "<email>",
    "password": "<password>"}
    ```
    in the body of `HTTP POST` request to the `api/v1/users/` endpoint.

2. Authenticate by passing email:pasword in base64 format with Basic authorization header to the `api/v1/users/auth` endpoint.
    And use the returned string as Basic authorization header for the following requests.
    
## Links shortening
1. Get existing or create a new short link by sending json with the following structure
    ```
    {"url": "<original url>"}
    ```
    in the body of `HTTP GET` request to the `api/v1/links/short/` endpoint.
2. Get an original link by sending json with the following structure
    ```
    {"url": "<short url>"}
    ```
    in the body of `HTTP GET` request to the `api/v1/links/original/` endpoint.
3. Get list of all created links with number of short link clicks by sending `HTTP GET` request to the `api/v1/links` endpoint.
