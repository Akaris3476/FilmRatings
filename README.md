# Film Ratings

FilmRatings is a backend application for storing and managing users ratings for movies
<br>
This application allows to browse films, submit ratings and view statistics like average score

## Endpoints
```
# Films
Get /films                  # Get films
Get /films?include=ratings  # Get films with ratings
Get /films?title            # Search films by title 
Get /films?page             # Specify page 

Get /films/{filmId}         # Get particular film 
Post /films/{filmId}        # Add new film
Put /films/{filmId}         # Update film
Delete /films/{filmId}      # Delete film

# Ratings
Get /films/{filmId}/ratings                 # Get all ratings for a film
Get /films/{filmId}/ratings?page            # Specify page

# Adds info about user to rating from JWT
Post /films/{filmId}/ratings                # Add new rating
# Only for rating's author or admin
Put /films/{filmId}/ratings/{ratingId}      # Update Rating
# Only for rating's author or admin
Delete /films/{filmId}/ratings/{ratingId}   # Delete Rating

# Authentication
Post /auth/register
Post /auth/login
Post /auth/refresh  
Post /auth/logout

# Users
Patch /users/{email}     # Update user's admin status
```


## Setup
```
docker run -p 6379:6379 --name redis -d redis
dotnet run
```
Also requires PostgreSQL
