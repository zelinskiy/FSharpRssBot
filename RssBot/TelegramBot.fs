module TelegramBot

open SettingsLoader
open Network


let token = SETTINGS.Item "Token"
let prefix = "https://api.telegram.org/bot" + token + "/"

let makeRequest (methodName:string) = Network.getAsync (prefix + methodName)

let test = makeRequest "getMe" |> Async.RunSynchronously

   
    
