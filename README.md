# Mcsa Meets Mailer

## Requirements

* Poll the meets spreadsheet for data.
* Transform meets data into emails.
* Send monthly and weekly mails.
* Support 'pretty' email format (html?) and printer friendly.
* Notify meet leader & specified committee person of 'invalid' entries (e.g. missing meet date).
* Ability to preview monthly & weekly mails.
* Ability to 'force' send monthly & weekly mails (e.g. in case correction was made).
* View log online, email on errors?
* Meet leader mailed a reminder of upcoming meet(s).
* Attendees can subscribe for meet updates (e.g. changes, reminders).
* Subscribed Attendees mailed a reminder of upcoming meet(s).

### Meet Details

* Start Date
* Start Time
* End Date (optional)
* End Time (optional)
* Title
* Description (optional)
* Grade
* Leader
* Leader email (optional if cell provided)
* Leader cell (optional if email provided)
* Venue
* Address (optional)
* GPS Location (optional)
* Notes (skills, gear requirements, etc.) (optional)

### Questions

* Meet Convenor could be sent a preview of each mail and required to approve before mail is sent?

## Use-cases

### Key

* Meet = Event, activity, etc.
* Leader = Meet Leader (person co-ordinating and present at the Meet)
* Convenor = Committee member reponsible for Meets.
* Attendee = Person attending a meet.

### Cases

* Leader adds Meet, provides valid input, receives confirmation.
* Leader adds Meet, provides invalid input, receives instructions to correct.
* Leader adds Meet, omits 'Leader Email', Convenor notified.
* Leader updates Meet, provides valid input, receives confirmation
* Leader updates Meet, provides invalid input, receives instructions to correct.
* Leader updates Meet, omits 'Leader Email', Convenor notified.
* Leader removes Meet, receives confirmation.
* Convenor receives confirmation of added/updated/removed Meet.
* Convenor receives notification of invalid Meets (delayed, give Leader time to correct).
* Convenor previews next monthly mail.
* Convenor previews next weekly mail.
* Convenor 'forces' re-send of monthly mail.
* Convenor 'forces' re-send of weekly mail.
* Attendee subscribes to Meet.
* Attendee unsubscribes from Meet.
* Leader receives reminder of upcoming meet(s).
* Subscribed Attendee receives reminder of upcoming meet(s).
* Members receive monthly mail.
* Members receive weekly mail.
* Maintainer views log via webpage.
* Maintainer receives email notification of errors.
