Config = {
     lan               = "en",
     PedRenderDistance = 80.0,
     target            = false,
     eventPrefix       = "mh"
}

Config.MHAdminSystem = GetResourceState("mate-admin") == "started"

Config.ApprovedLicenses = {
     "license:123",
     "discord:123",
}


ServerOnly = {}
