module TelegramBot

open SettingsLoader

open Microsoft.FSharp.Control.CommonExtensions   
open System.Net
open System.IO


let token = SETTINGS.Item "Token"
let prefix = "https://api.telegram.org/bot" + token + "/"

let makeUrlRequest (url:string) =        
    async {
        let req = WebRequest.Create(url)       
        use! resp = req.AsyncGetResponse()
        use stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let html =  reader.ReadToEnd()
        printfn "%s %d" url html.Length
        return html
    }

let makeRequest (methodName:string) = makeUrlRequest (prefix + methodName)


let test = makeRequest "getMe" |> Async.RunSynchronously

   
    
