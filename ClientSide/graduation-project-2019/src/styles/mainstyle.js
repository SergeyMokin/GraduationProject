import React, {
  StyleSheet
} from 'react-native';

module.exports = StyleSheet.create({
  container: {
    alignItems: 'stretch',
    flex: 1
  },
  body: {
    flex: 9,
    flexDirection:'row',
    alignItems:'center',
    justifyContent:'center',
    backgroundColor: 'white',
  },
  primaryButton: {
    margin: 10,
    padding: 15,
    backgroundColor:"blue",
    alignSelf: 'auto',
    justifyContent:'center'
  }
});