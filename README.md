# Project_sample
quiz 1 and 2 


nag create ako ng seperate file para maging optimize and readable

hi im using vscode for this project hopefully you run this project file on vscode to run correctly

IF EVER NA SA TINGIN NYO WALANG NAGBAGO JUST SCROLL THE TERMINAL PATAAS AND MAKIKITA NYO DON NATATABUNAN KASE MINSAN THANKS 


KUNG VSCODE YUNG GAGAMITIN FOR THIS IS DAPAT SA Bash terminal ILAGAY LANG YUNG DOTNET BUILD or kung may error na sabi is project_sample doesnt not exist sa bash terminal mag dotnet clean muna tapos isabay na yung dotnet build pag nag successfull, sabay irun yung Program.cs

tapos need muna mag install ng .net para mapagana yung program if ever sa vscode irurun thats why i suggest sa vscommunity nalang kaso kase wala nang extra space para sa ma download yung ibang pang resources kaya dito ko nalang ginawa 


features of my shopping cart system part 2 

1. Search Product by Name I added a search feature so users can quickly find a specific food item without scrolling through the entire menu. Instead of browsing all items, the user can just type a keyword and instantly see matching results — making navigation faster and more convenient especially as the menu grows longer.

2. Manage and Update Cart After the basic add-to-cart was working, I realized users needed more control over their cart before proceeding to checkout. So I added a full cart management sub-menu — allowing users to view their cart, update item quantities, remove a specific item, or clear the entire cart. This made the shopping experience more realistic and flexible.

3. Checkout Once cart management was stable, I added the checkout feature. This included computing the grand total, applying a 10% discount automatically when the purchase reaches PHP 5,000, validating the payment amount in a loop until it is sufficient, computing the change, and generating a receipt with a receipt number and timestamp.

4. Order History Finally, I added an order history feature so users can look back at all completed transactions within the same session. Each completed checkout is saved as an Order object — capturing the receipt number, timestamp, items purchased, totals, payment, and change. Users can also reprint any past receipt by entering its receipt number.

....




AI Prompt

1. With the help of AI, I ask ai to applied colors to validation and success messages and added visual separators to improve the console UI.

2. i asked ai to debugged and corrected the updated stock quantity behavior. i noticed that updating a cart item's quantity wasn't reflecting correctly in the remaining stock. After identifying where the problem was, i aske AI  to correct it. The root cause was that the old quantity was never being restored back to stock before applying the new one:


3. with the help of ai i finally fix the logic of remove cart and clear cart i found that the logic and identified the missing step of the correct placement of RestoreStock() in both case


4. i asked the ai to help me debug the order history because it does not reflect on the terminal after the purchases was being made After a long investigating,the main reason is the cart snapshot was being taken after the cart was already cleared, meaning the Order object was being saved with an empty Items array:

5. i asked ai to help me in terms of search product by name because it will show on the terminal that it was not found thats why i asked ai to help me on this . the main cause is because it was a case sensitivity mismatch combined with accidental whitespace in the search input. The search was comparing the raw user input directly against the product names without normalizing both sides first: