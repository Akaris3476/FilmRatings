# Film Ratings

FilmRatings is a backend application for storing and managing users ratings for movies
<br>
This application allows to browse films, submit ratings and view statistics like average score

## Endpoints
```
Get /films                  # Get all films
Get /films?include=ratings  # Get all films with ratings
Get /films/{filmId}         # Get particular film 
Get /films?include=ratings  # Get film with ratings
Put /films/{filmId}         # Update film
Delete /films/{filmId}      # Delete film


Get /films/{filmId}/ratings                 # Get all ratings for a film
Post /films/{filmId}/ratings                # Add new rating
Put /films/{filmId}/ratings/{ratingId}      # Update Rating
Delete /films/{filmId}/ratings/{ratingId}   # Delete Rating

Post /auth/register
Post /auth/login
```


## Setup
```
docker run -p 6379:6379 --name redis -d redis
dotnet run
```

