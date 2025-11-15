namespace FilmRatings.Core.Models;

public class User
{
	public Guid Id { get; set; }
	public string Username { get; private set; }
	public string Email { get; private set; }
	
	public bool IsAdmin { get; private set; }
	public string HashedPassword { get; set; }
	public List<Rating> Ratings { get; set; } = new List<Rating>();
	

	public User(Guid id, string email, string username, string password, bool isAdmin = false)
	{

		Id = id;
		SetEmail(email);
		SetUsername(username);
		HashedPassword = password;
		IsAdmin = isAdmin;
	}

	public void SetUsername(string username)
	{
		if (username.Length > 20 || username.Length < 3)
			throw new ArgumentException("Username must be between 3 and 20 characters");
		
		Username = username;
	}
	
	public void SetEmail(string email)
	{
		if (!email.Contains("@"))
			throw new ArgumentException("Email must contain @");
		
		Email = email;
	}

	public void SetRatings(List<Rating> ratings)
	{
		Ratings = ratings;
	}

}

