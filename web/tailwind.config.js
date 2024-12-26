/** @type {import('tailwindcss').Config} */
const color = require('tailwindcss/colors');
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme:{
    extend: {
      colors:{
        'nepal-blue': '#003893',
        'nepal-dark-blue': '#022763',
        'nepal-red':'#DC143C'
      }
    },
    // color:{
    //   red:'#003893'
    // }
  },
  plugins: [],
}
