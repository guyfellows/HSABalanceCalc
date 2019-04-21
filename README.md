# HSABalanceCalc

This project captures data about a user's personal information and then processes 5 years (60 months) of random events to calculate HSA contribution and distributions.

- Tracks YTD maximums against self and family contribution limits for all five years
- Tracks any out-of-pocket expenses when users have insufficient HSA balance for distribution

The events.txt provides an event for all 60 months. The file location needs to be adjusted in the User.cs to the location of the file (Line 199)
