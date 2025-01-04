<template>
  <div class="text-gray-600 mb-8 mx-4">
    <p class="font-medium text-lg mb-2">Voters Details</p>
  </div>

  <div class="lg:col-span-2">
    <div class="mx-6 my-8">

      <div class="flex space-x-12 mb-4 p-4">
        <div class="flex-1">
          <label for="fullname" class="block text-gray-700 font-medium mb-2">Full Name</label>
          <input
            type="text"
            name="fullname"
            id="fullname"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="John Doe"
          />
        </div>
        <div class="flex-1">
          <label for="dob" class="block text-gray-700 font-medium mb-2">Date of Birth</label>
          <input
            type="date"
            name="dob"
            id="dob"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
          />
        </div>
      </div>

      <div class="flex space-x-12 mb-4 p-4">
        <div class="flex-1">
          <label for="fathername" class="block text-gray-700 font-medium mb-2">Father's Name</label>
          <input
            type="text"
            name="fathername"
            id="fathername"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="John Sr."
          />
        </div>
        <div class="flex-1">
          <label for="mothername" class="block text-gray-700 font-medium mb-2">Mother's Name</label>
          <input
            type="text"
            name="mothername"
            id="mothername"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="Jane Doe"
          />
        </div>
      </div>

      <!-- Grandfather's Name and Grandmother's Name (2 Fields in One Row) -->
      <div class="flex space-x-12 mb-4 p-4">
        <div class="flex-1">
          <label for="grandfathername" class="block text-gray-700 font-medium mb-2">Grandfather's Name</label>
          <input
            type="text"
            name="grandfathername"
            id="grandfathername"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="James Doe"
          />
        </div>
        <div class="flex-1">
          <label for="grandmothername" class="block text-gray-700 font-medium mb-2">Grandmother's Name</label>
          <input
            type="text"
            name="grandmothername"
            id="grandmothername"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="Mary Doe"
          />
        </div>
      </div>

      <!-- Address and Phone Number (2 Fields in One Row) -->
      <div class="flex space-x-12 mb-4 p-4">
        <div class="flex-1">
          <label for="address" class="block text-gray-700 font-medium mb-2">Address</label>
          <input
            type="text"
            name="address"
            id="address"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="123 Main Street"
          />
        </div>
        <div class="flex-1">
          <label for="phonenumber" class="block text-gray-700 font-medium mb-2">Phone Number</label>
          <input
            type="tel"
            name="phonenumber"
            id="phonenumber"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="123-456-7890"
          />
        </div>
      </div>

      <!-- Email and Password (2 Fields in One Row) -->
      <div class="flex space-x-12 mb-4 p-4">
        <div class="flex-1">
          <label for="email" class="block text-gray-700 font-medium mb-2">Email Address</label>
          <input
            type="email"
            name="email"
            id="email"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="email@domain.com"
          />
        </div>
        <div class="flex-1">
          <label for="password" class="block text-gray-700 font-medium mb-2">Password</label>
          <input
            type="password"
            name="password"
            id="password"
            class="h-10 border mt-1 rounded px-4 w-full bg-gray-50 focus:ring focus:ring-blue-300"
            placeholder="********"
          />
        </div>
      </div>

      <!-- Submit Button -->
      <div class="text-right mx-2">
        <button
          class="bg-[#003893] hover:bg-[#002b73] text-white font-bold py-2 px-6 rounded"
        >
          Submit
        </button>
      </div>

    </div>
  </div>
</template>
