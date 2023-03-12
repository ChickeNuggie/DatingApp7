namespace API.Extensions
{
    // static = can only contain static members, such as static methods and static properties.
    //often used to group related utility functions or to provide a global point of access for shared resources.
    // In this case, static used where method is accessable only to this class
    // Access methods in class without creating new instances of the class 
    //i.e. DateTimeExtension.calculateage instead of 'var caculate = new DateTimeExtensions();'
    public static class DateTimeExtensions 
    {
        public static int CalculateAge(this DateOnly dob) 
        { 
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age)) age--; // decrease one year from dob if yet to pass their birth dates.

            return age;
         }
    }
}