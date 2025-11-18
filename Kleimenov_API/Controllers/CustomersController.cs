using Microsoft.AspNetCore.Mvc;
using Kleimenov_API.Services;
using Kleimenov_API.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Kleimenov_API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _customerService;

    public CustomersController(CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllCustomersAsync();

        var response = customers.Select(customer => new CustomerDto
        {
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address
        }).ToList();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null) 
            return NotFound();

        var response = new CustomerDto
        {
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address
        };
        return Ok(response);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customerDto)
    {
        var created = await _customerService.CreateCustomerAsync(
            customerDto.FullName,
            customerDto.Email,
            customerDto.Phone,
            customerDto.Address
        );

        return CreatedAtAction(nameof(GetCustomer), new { id = created.CustomerId }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
    {
        await _customerService.UpdateCustomerAsync(id, customerDto.FullName, customerDto.Email, customerDto.Phone, customerDto.Address);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        await _customerService.DeleteCustomerAsync(id);
        return NoContent();
    }
}
