module TelegramBot

open SettingsLoader
open Network

open System
open FSharp.Data
open FSharp.Data.JsonExtensions

let token = SETTINGS.Item "Token"
let prefix = "https://api.telegram.org/bot" + token + "/"

let makeParamsString (pars:list<string*string>) =
    pars
    |> Seq.map (fun (a, b) -> a + "=" + b)
    |> (fun s -> String.Join("&", s))
    |> (fun s -> "?" + s)

let makeRequest (methodName:string) (pars:list<string*string>) = 
    Network.getAsync (prefix + methodName + makeParamsString pars)

let info = JsonValue.Parse(""" 
    { "name": "Tomas", "born": 1985,
      "siblings": [ "Jan", "Alexander" ] } """)


let test = 
    makeRequest "getUpdates" [] 
    |> Async.RunSynchronously     
    |> JsonValue.Parse
    |> (fun x -> x?result)
    |> JsonExtensions.AsArray
    |> Seq.map (fun x -> x?update_id.AsString())
    |> (fun seq -> String.Join("\n", seq))
   
    
