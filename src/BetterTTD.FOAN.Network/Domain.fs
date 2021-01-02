namespace BetterTTD.FOAN.Network

module Domain =

    open BetterTTD.FOAN.Network.Enums
    
    type Client =
        { Name           : string
          CompanyId      : int
          Language       : NetworkLanguage
          NetworkAddress : string }
        
    type Company =
        { Name         : string
          President    : string
          Inaugurated  : int64
          Value        : int64
          Income       : int64
          Performance  : int
          IsPassworded : bool
          IsAI         : bool
          Color        : Color
          Vehicles     : Map<VehicleType, int>
          Stations     : Map<VehicleType, int> }