# Virtual Regatta - Technical test

Technical test I've done for an application to a job with Virtual Regatta.

This was a hassle as I don't know Microsoft Orleans at all and the subject was very vague.

-----

## Questions answering

> The given `TodoItem item` parameter of `TodoController.Create` isn't good practice.

It isn't good practice to directly feed the deserialized object to our business code. We should rather either pass the attributes to create the object, or check for the object's attribute's validity and return an error in case of a bad request.

> There's no response from `TodoController.Create` which isn't good practice too.

Replying to the request only with a 200 HTTP code isn't definitely not the best practice. Returning with the new item's GUID is better practice, as well returning accurate error codes when the input item is bad.

-----

I added **Swagger** on the API, as I find project documentation pretty important.
Check it here : http://localhost:5079/swagger