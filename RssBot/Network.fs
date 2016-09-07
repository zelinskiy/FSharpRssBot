module Network

open Microsoft.FSharp.Control.CommonExtensions   
open System.Net
open System.IO

let getAsync (url:string) =        
    async {
        let req = WebRequest.Create(url)       
        use! resp = req.AsyncGetResponse()
        use stream = resp.GetResponseStream()
        use reader = new StreamReader(stream)
        let html =  reader.ReadToEnd()
        return html
    }