namespace Shared

type Data = { Name: string; X1: float; X2: float; X3: float; X4: float; X5: float }
type Values = Data []

module Route =
    /// Defines how routes are generated on server and mapped from client
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

/// A type that specifies the communication protocol between client and server
/// to learn more, read the docs at https://zaid-ajaj.github.io/Fable.Remoting/src/basics.html
type IDemoApi =
    { demoValues : int -> Async<Values> }
