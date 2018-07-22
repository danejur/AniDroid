# Build Requirements

In order for the app to build, you will need to create two files not included in the repository. In addition, you will also need to create an API client on AniList (https://anilist.co/settings/developer)

### AppConfig.xml

This will go in the `values` directory, which is located in the `Resources` directory inside the `AniDroid` project. This file will contain AniList API access data, as well as what the App name will be in both debug and release builds. The following is an example of this file:

```html
<?xml version="1.0" encoding="utf-8" ?>
<resources>
  <item name="AppName" type="config">AniDroid</item>
  <item name="AppNameDebug" type="config">AniDroid Debug</item>

  <item name="ApiClientId" type="config">YOUR_ANILIST_CLIENT_ID</item>
  <item name="ApiClientSecret" type="config">YOUR_ANILIST_CLIENT_SECRET</item>
  <item name="ApiRedirectUri" type="config">YOUR_ANILIST_REDIRECT_URI</item>
</resources>
```

### google-services.json

Work in progress, will finish later