namespace firstGiraffe.Models

[<CLIMutable>]
type Message =
    {
        Text : string
    }

[<CLIMutable>]
type QueryParameters =
    {
        url  : string
        checkout : string
        checkin: string 
        adult :string 
        currency: string
    }