# cs2-gift-packages

**This plugin makes it possible to drop players gifts!**

<br>

<details>
	<summary>showcase</summary>
	
https://github.com/user-attachments/assets/f38cc097-9c0b-44d6-9adf-3e3d0a93f0a4
	
</details>

<br>

## information:

### requirements

- [MetaMod](https://github.com/alliedmodders/metamod-source)
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)
- [MultiAddonManager](https://github.com/Source2ZE/MultiAddonManager)

<br>

## example config
these can be used in command string
> {NAME} {STEAMID} {STEAMID3} {STEAMID64} {USERID} {SLOT}
>
> {GIFTER_NAME} {GIFTER_STEAMID} {GIFTER_STEAMID3} {GIFTER_STEAMID64} {GIFTER_USERID} {GIFTER_SLOT}
```json
{
  "Prefix": "{Magenta}[Gifts]{bluegrey}",
  "Permission": [ "@css/root", "#css/admin" ],
  "Command": [ "css_giftpackage", "css_giftmenu" ],
  "Gifts": [
    {
      "Name": "Example",
      "Model": "models/therazu/props/gift/gift.vmdl",
      "Announce": true,
      "Quiet": false,
      "SoundEvent": "UIPanorama.gift_claim",
      "Command": [ "css_example {STEAMID64} 1" ]
    }
  ]
}
```

<br> <a href="https://ko-fi.com/exkludera" target="blank"><img src="https://cdn.ko-fi.com/cdn/kofi5.png" height="48px" alt="Buy Me a Coffee at ko-fi.com"></a>