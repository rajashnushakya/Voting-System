<template>
  <div class="min-h-full">
    <Disclosure as="nav" class="bg-nepal-blue" v-slot="{ open }">
      <div class="mx-auto px-4 sm:px-6 lg:px-8">
        <div class="flex h-16 items-center justify-between">
          <div class="flex items-center">
            <div class="shrink-0">
              <img class="size-8" src="https://tailwindui.com/plus/img/logos/mark.svg?color=indigo&shade=500" alt="Your Company" />
            </div>
            <div class="hidden md:block">
              <div class="ml-10 flex items-baseline space-x-4">
                <template v-for="item in navigation" :key="item.name">
                  <template v-if="!item.children">
                    <a 
                      :href="item.href" 
                      :class="{
                        'bg-nepal-dark-blue': item.current, 
                        'text-white rounded-md px-3 py-2 text-sm font-medium': true, 
                        'hover:bg-nepal-dark-blue': !item.current
                      }" 
                      :aria-current="item.current ? 'page' : undefined" 
                      @click="setCurrentItem(item)"
                    >
                      {{ item.name }}
                    </a>
                  </template>
                  <Menu as="div" v-else class="relative inline-block text-left">
                    <div>
                      <MenuButton class="inline-flex w-full justify-center gap-x-1.5 rounded-md px-3 py-2 text-sm font-medium text-white hover:bg-nepal-dark-blue">
                        {{ item.name }}
                        <ChevronDownIcon class="-mr-1 size-5 text-white" aria-hidden="true" />
                      </MenuButton>
                    </div>
                    <transition enter-active-class="transition ease-out duration-100" enter-from-class="transform opacity-0 scale-95" enter-to-class="transform opacity-100 scale-100" leave-active-class="transition ease-in duration-75" leave-from-class="transform opacity-100 scale-100" leave-to-class="transform opacity-0 scale-95">
                      <MenuItems class="absolute left-0 z-10 mt-2 w-56 origin-top-right rounded-md bg-white shadow-lg ring-1 ring-black/5 focus:outline-none">
                        <div class="py-1">
                          <MenuItem v-for="subItem in item.children" :key="subItem.name" v-slot="{ active }">
                            <a :href="subItem.href" :class="{
                              'bg-gray-100 text-gray-900 outline-none': active,
                              'text-gray-700': !active,
                              'block px-4 py-2 text-sm': true
                            }" @click="setCurrentItem(subItem)">
                              {{ subItem.name }}
                            </a>
                          </MenuItem>
                        </div>
                      </MenuItems>
                    </transition>
                  </Menu>
                </template>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Disclosure>

    <AddElection :dialogActive="dialogActive" @update:dialogActive="dialogActive = $event" />
    <AddCentre v-model:dialog="showDialog" />
    <AddElectionCentre v-model:EdialogActive="EdialogActive" />
    <AddCandidate v-model:CdialogActive="CdialogActive" />
    <ChangeCredentials :settingActive="settingActive" @update:settingActive="settingActive = $event" />

    <CandidateCentre v-model:ECdialogActive="ECdialogActive" />



  </div>
</template>
<script setup>
import { ref } from 'vue';
import AddElection from './AddElection.vue'; 
import AddCentre from './AddCentre.vue';
import AddElectionCentre from './AddElectionCentre.vue';
import { Disclosure, Menu, MenuButton, MenuItem, MenuItems } from '@headlessui/vue';
import { ChevronDownIcon } from '@heroicons/vue/20/solid';
import AddCandidate from './AddCandidate.vue';
import ChangeCredentials  from './ChangeCredentials.vue';
import CandidateCentre from './CandidateCentre.vue';

const dialogActive = ref(false);
const showDialog = ref(false);
const  EdialogActive= ref(false);
const CdialogActive = ref(false);
const settingActive = ref(false);
const ECdialogActive = ref(false);

const navigation = [
  { name: 'Dashboard', href: 'dashboard', current: false },
  {
    name: 'Election',
    href: '#',
    current: false,
    children: [
      { name: 'Add Election'},
      { name: 'Add Centre' },
      { name: 'Add Election Centre'},
      {name: 'All Election Details'}
    ],
  },
  { name: 'Result', href: 'result', current: false },
  { name: 'Setting', href: '#', current: false, children:[
    {name: 'Change Password'},
    {name: 'Logout'}
  ],
},
  {name: 'Candidate', 
  href: '#', 
  current: false,
  children: [
    {name: 'Add Candidate'},
    {name: 'Enroll Candidate'}
  ]},
];

function setCurrentItem(item) {
  navigation.forEach(navItem => {
    navItem.current = navItem === item;

    if (navItem.children) {
      navItem.children.forEach(subItem => {
        subItem.current = subItem === item;
      });
    }
  });

  if (item.name === 'Add Election') {
    dialogActive.value = true;
  }
  if (item.name === 'Add Centre') {
    showDialog.value = true;
  }
  if (item.name === 'Add Election Centre') {
    EdialogActive.value = true;
  }
  if (item.name === 'Add Candidate') {
    CdialogActive.value = true;
  }
  if (item.name === 'Enroll Candidate') {
    ECdialogActive.value = true;
  }
  if (item.name === 'Change Password') {
    settingActive.value = true;
    console.log('Change Credentials clicked');
  }
  if (item.name === 'All Election Details') {
    window.location.href = '/election/detail'; 

  }
  if (item.name === 'Logout') {
    localStorage.removeItem('token');
    localStorage.removeItem('voterid');
    window.location.href = '/login';
  }
}
</script>

