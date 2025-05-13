# Fare

API to query train fares.

## Usage

```powershell
# All possible travel distance permutations
Invoke-WebRequest "https://mrt.from.sg/All"

# Get distance and fares for specific stations
Invoke-WebRequest "https://mrt.from.sg/Distance?from=Upper Thomson&to=Woodlands"
```

## Development

Provide env `STN_DATA` which is copy pasted from this page

https://sgwiki.com/index.php?title=List_of_Distance_between_Stations&action=edit