//make it necessary to declare in the class.
//define a structure for an object.
//can help to catch errors early and prevent issues at runtime.
//makes the code more readable and easier to maintain, as the structure of objects is clearly defined and documented
//sed to enforce consistency across different parts of an application that use the same data structures, making it easier to manage changes and updates over time.
export interface Photo {
    id: number;
    url: string;
    isMain: boolean;
    isApproved: boolean;
    username?: string;
    
}
