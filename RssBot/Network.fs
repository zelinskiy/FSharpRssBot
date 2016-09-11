module Network

open Microsoft.FSharp.Control.CommonExtensions   
open System.Net
open System.IO

let makeParamsString (pars:list<string*string>) =
    pars
    |> Seq.map (fun (a, b) -> a + "=" + b)
    |> (fun s -> String.Join("&", s))
    |> (fun s -> "?" + s)

let private makeRequest (metod:string) (url:string)=        
    async {
        let req = WebRequest.Create(url)
        req.Method = metod |> ignore
        use! resp = req.AsyncGetResponse()
        use stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let html =  reader.ReadToEnd()
        return html
    }

let getAsync = makeRequest "GET"

let postAsync = makeRequest "POST"